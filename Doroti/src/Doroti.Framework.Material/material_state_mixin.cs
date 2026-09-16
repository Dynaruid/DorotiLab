// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material_state_mixin.dart
namespace Doroti.Framework.Material;

public interface MaterialStateMixin<T> where T : StatefulWidget
{
    HashSet<WidgetState> materialStates { get; set; }

    public System.Action<bool> updateMaterialState(WidgetState key, System.Action<bool>? onChanged = null);
    public void setMaterialState(WidgetState state, bool isSet);
    public void addMaterialState(WidgetState state);
    public void removeMaterialState(WidgetState state);
    public bool isDisabled { get; }
    public bool isDragged { get; }
    public bool isErrored { get; }
    public bool isFocused { get; }
    public bool isHovered { get; }
    public bool isPressed { get; }
    public bool isScrolledUnder { get; }
    public bool isSelected { get; }
    public void debugFillProperties(DiagnosticPropertiesBuilder properties);
}
