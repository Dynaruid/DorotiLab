// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material_state_mixin.dart
namespace Doroti.Framework.Material;

public interface MaterialStateMixin<T> where T : global::Doroti.Framework.Widgets.StatefulWidget
{
    HashSet<global::Doroti.Framework.Widgets.WidgetState> materialStates { get; set; }

    public global::System.Action<bool> updateMaterialState(global::Doroti.Framework.Widgets.WidgetState key, global::System.Action<bool>? onChanged = null);
    public void setMaterialState(global::Doroti.Framework.Widgets.WidgetState state, bool isSet);
    public void addMaterialState(global::Doroti.Framework.Widgets.WidgetState state);
    public void removeMaterialState(global::Doroti.Framework.Widgets.WidgetState state);
    public bool isDisabled { get; }
    public bool isDragged { get; }
    public bool isErrored { get; }
    public bool isFocused { get; }
    public bool isHovered { get; }
    public bool isPressed { get; }
    public bool isScrolledUnder { get; }
    public bool isSelected { get; }
    public void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties);
}
