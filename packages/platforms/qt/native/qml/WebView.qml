import QtQuick
import QtWebEngine
import QtWebChannel

WebEngineView {
    id: view
    objectName: "doroti-quick-webview"
    required property var bridge
    profile: bridge.profile
    settings.localContentCanAccessFileUrls: false
    settings.localContentCanAccessRemoteUrls: false
    settings.javascriptCanOpenWindows: false
    settings.fullScreenSupportEnabled: false
    settings.screenCaptureEnabled: false
    settings.focusOnNavigationEnabled: false
    webChannelWorld: WebEngineScript.ApplicationWorld
    webChannel: WebChannel { id: channel }
    userScripts.collection: [{
        name: "doroti-trusted-app-transport",
        injectionPoint: WebEngineScript.DocumentReady,
        worldId: WebEngineScript.ApplicationWorld,
        runsOnSubFrames: false,
        sourceCode: view.bridge.transportScript
    }]
    onNavigationRequested: function(request) {
        if (!bridge.allows(request.url.toString())) request.reject()
    }
    onLoadingChanged: function(info) { bridge.loading(info.status, info.errorString) }
    onRenderProcessTerminated: function(status, code) { bridge.failed("Renderer terminated: " + status + "/" + code) }
    onPermissionRequested: function(permission) { permission.deny() }
    onCertificateError: function(error) { error.rejectCertificate() }
    onFileDialogRequested: function(request) { request.reject() }
    onJavaScriptDialogRequested: function(request) { request.reject() }
    onAuthenticationDialogRequested: function(request) { request.reject() }
    onFullScreenRequested: function(request) { request.reject() }
    onRegisterProtocolHandlerRequested: function(request) { request.reject() }
    function dispatch(operation, text, request) {
        switch (operation) {
        case 2: url = text; break
        case 3: loadHtml(text); break
        case 4: reload(); break
        case 5: stop(); break
        case 6: goBack(); break
        case 7: goForward(); break
        case 8:
            runJavaScript(text, function(value) { bridge.scriptResult(request, value) })
            break
        }
    }
    function installFacade(generation) {
        runJavaScript("(()=>{if(location.origin!=='doroti-app://content')return;let id=0;window.doroti={postMessage:(name,payload)=>window.dispatchEvent(new CustomEvent('doroti-message',{detail:JSON.stringify({generation:" + generation + ",requestId:++id,name,payload})}))};})()")
    }
    Component.onCompleted: { channel.registerObject("doroti", bridge.transport); loadHtml(bridge.initialHtml) }
}
