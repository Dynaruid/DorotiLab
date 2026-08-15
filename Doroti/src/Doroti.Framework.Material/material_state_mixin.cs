// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material_state_mixin.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public interface MaterialStateMixin<T> where T : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> materialStates { get; set; }

    public global::System.Action<bool> updateMaterialState(global::Doroti.Generated.Framework.Widgets.WidgetState key, global::System.Action<bool>? onChanged = null);
    public void setMaterialState(global::Doroti.Generated.Framework.Widgets.WidgetState state, bool isSet);
    public void addMaterialState(global::Doroti.Generated.Framework.Widgets.WidgetState state);
    public void removeMaterialState(global::Doroti.Generated.Framework.Widgets.WidgetState state);
    public bool isDisabled { get; }
    public bool isDragged { get; }
    public bool isErrored { get; }
    public bool isFocused { get; }
    public bool isHovered { get; }
    public bool isPressed { get; }
    public bool isScrolledUnder { get; }
    public bool isSelected { get; }
    public void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties);
}
