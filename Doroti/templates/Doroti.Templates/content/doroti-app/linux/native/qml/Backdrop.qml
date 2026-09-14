import QtQuick

// The source group contains only earlier paint-order siblings. Neither this
// effect nor its sharp foreground child can enter the live texture dependency.
Item {
    id: root
    property Item background
    property rect sampleRect
    property rect outputRect
    property real sigma: 1
    clip: true
    x: outputRect.x
    y: outputRect.y
    width: outputRect.width
    height: outputRect.height

    ShaderEffectSource {
        id: capture
        sourceItem: root.background
        sourceRect: root.sampleRect
        width: root.sampleRect.width
        height: root.sampleRect.height
        live: true
        recursive: false
        hideSource: false
        visible: false
    }
    ShaderEffect {
        id: horizontal
        width: root.sampleRect.width
        height: root.sampleRect.height
        property variant source: capture
        property vector2d stepSize: Qt.vector2d(1 / width, 0)
        property real sigma: root.sigma
        fragmentShader: "qrc:/doroti/shaders/gaussian.frag.qsb"
        layer.enabled: true
        layer.smooth: true
        visible: false
    }
    ShaderEffect {
        x: root.sampleRect.x - root.outputRect.x
        y: root.sampleRect.y - root.outputRect.y
        width: root.sampleRect.width
        height: root.sampleRect.height
        property variant source: horizontal
        property vector2d stepSize: Qt.vector2d(0, 1 / height)
        property real sigma: root.sigma
        fragmentShader: "qrc:/doroti/shaders/gaussian.frag.qsb"
    }
}
