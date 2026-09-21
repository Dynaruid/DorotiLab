// Optional GStreamer 1.24+ adapter. No gst_buffer_map/readback fallback.
#include <gst/gst.h>
#include <gst/app/gstappsink.h>
#include <gst/allocators/gstdmabuf.h>
#include <gst/video/video.h>
#include <gst/video/video-info-dma-drm.h>
#include <cstdint>
#include <unistd.h>
#include <cstdio>
#include <cstring>
#include <memory>
#include <stdexcept>
#include <string>
#define EXPORT extern "C" __attribute__((visibility("default")))
namespace {
struct Camera {
  GstElement* pipeline=nullptr; GstAppSink* sink=nullptr;
  ~Camera() { if(pipeline)gst_element_set_state(pipeline,GST_STATE_NULL); if(sink)gst_object_unref(sink); if(pipeline)gst_object_unref(pipeline); }
};
void Error(char* buffer,uint32_t size,const char* value) { if(buffer && size)std::snprintf(buffer,size,"%s",value); }
std::string Message(GstMessage* message) {
  GError* error=nullptr; gchar* debug=nullptr; gst_message_parse_error(message,&error,&debug);
  std::string value=error?error->message:"GStreamer error"; if(error)g_error_free(error);g_free(debug);return value;
}
}
struct DorotiGstFrame { void* sample; int32_t fd; uint32_t width,height,format; uint64_t allocation_size,offset,row_pitch,modifier; };
static_assert(sizeof(DorotiGstFrame)==56);
EXPORT int doroti_texture_gst_open(const char* description,void** result,char* error,uint32_t size) noexcept {
  try {
    if(!description || !result)throw std::invalid_argument("Missing pipeline/result"); *result=nullptr;
    GError* init_error=nullptr;
    if(!gst_init_check(nullptr,nullptr,&init_error)) {
      std::string message=init_error?init_error->message:"gst_init failed"; if(init_error)g_error_free(init_error);throw std::runtime_error(message);
    }
    auto camera=std::make_unique<Camera>();
    GError* parse_error=nullptr; camera->pipeline=gst_parse_launch(description,&parse_error);
    if(parse_error) { std::string message=parse_error->message;g_error_free(parse_error);throw std::runtime_error(message); }
    if(!camera->pipeline || !GST_IS_PIPELINE(camera->pipeline))throw std::runtime_error("A GStreamer pipeline is required");
    auto* sink=gst_bin_get_by_name(GST_BIN(camera->pipeline),"doroti_texture");
    if(!sink || !GST_IS_APP_SINK(sink)) { if(sink)gst_object_unref(sink);throw std::runtime_error("appsink name=doroti_texture is required"); }
    camera->sink=GST_APP_SINK(sink);
    gst_app_sink_set_max_buffers(camera->sink,2);gst_app_sink_set_drop(camera->sink,TRUE);gst_app_sink_set_wait_on_eos(camera->sink,FALSE);
    // Reject system-memory negotiation before pulling, and check each GstMemory as well.
    auto* caps=gst_caps_from_string("video/x-raw(memory:DMABuf),format=DMA_DRM");gst_app_sink_set_caps(camera->sink,caps);gst_caps_unref(caps);
    if(gst_element_set_state(camera->pipeline,GST_STATE_PLAYING)==GST_STATE_CHANGE_FAILURE)throw std::runtime_error("Pipeline could not start");
    *result=camera.release();return 0;
  } catch(const std::exception& e) { Error(error,size,e.what());return -1; }
}
EXPORT int doroti_texture_gst_read(void* value,DorotiGstFrame* result,char* error,uint32_t size) noexcept {
  GstSample* sample=nullptr;
  try {
    if(!value || !result)throw std::invalid_argument("Missing camera/result"); *result={};auto& camera=*static_cast<Camera*>(value);
    sample=gst_app_sink_try_pull_sample(camera.sink,100*GST_MSECOND);
    if(!sample) {
      auto* bus=gst_element_get_bus(camera.pipeline);auto* message=gst_bus_pop_filtered(bus,GST_MESSAGE_ERROR);gst_object_unref(bus);
      if(message){auto text=Message(message);gst_message_unref(message);throw std::runtime_error(text);}
      if(gst_app_sink_is_eos(camera.sink))throw std::runtime_error("Camera/video reached EOS");return 0;
    }
    auto* caps=gst_sample_get_caps(sample);auto* buffer=gst_sample_get_buffer(sample);
    GstVideoInfoDmaDrm info;gst_video_info_dma_drm_init(&info);
    if(!caps || !buffer || !gst_video_info_dma_drm_from_caps(&info,caps) || gst_buffer_n_memory(buffer)!=1)
      throw std::runtime_error("Expected one DMA_DRM memory allocation");
    auto* memory=gst_buffer_peek_memory(buffer,0);auto* meta=gst_buffer_get_video_meta(buffer);
    if(!gst_is_dmabuf_memory(memory) || !meta || meta->n_planes!=1 || meta->stride[0]<=0)
      throw std::runtime_error("Expected DMA-BUF with one positive-stride video plane");
    // DRM fourcc AB24=RGBA bytes, AR24=BGRA bytes on little-endian Linux.
    constexpr uint32_t AB24=0x34324241, AR24=0x34325241;
    if(info.drm_fourcc!=AB24 && info.drm_fourcc!=AR24)throw std::runtime_error("GPU-convert camera YUV to AB24/AR24 before appsink");
    gsize memory_offset=0,max_size=0;gst_memory_get_sizes(memory,&memory_offset,&max_size);
    result->sample=sample;result->fd=gst_dmabuf_memory_get_fd(memory);result->width=meta->width;result->height=meta->height;
    result->format=info.drm_fourcc==AB24?0:1;const auto allocation=lseek(result->fd,0,SEEK_END);
    if(allocation<=0)throw std::runtime_error("Could not query DMA-BUF allocation size");
    result->allocation_size=static_cast<uint64_t>(allocation);result->offset=memory_offset+meta->offset[0];
    result->row_pitch=static_cast<uint64_t>(meta->stride[0]);result->modifier=info.drm_modifier;
    return 1; // sample owns pool lease until final Vulkan retirement
  } catch(const std::exception& e) { if(sample)gst_sample_unref(sample);Error(error,size,e.what());return -1; }
}
EXPORT void doroti_texture_gst_release(void* sample) noexcept { if(sample)gst_sample_unref(static_cast<GstSample*>(sample)); }
EXPORT void doroti_texture_gst_close(void* camera) noexcept { delete static_cast<Camera*>(camera); }
