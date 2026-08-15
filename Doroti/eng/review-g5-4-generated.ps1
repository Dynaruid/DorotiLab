#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)]
    [string] $OutputRoot
)

$ErrorActionPreference = 'Stop'
$materialRoot = Join-Path ([IO.Path]::GetFullPath($OutputRoot)) 'projects/Material'
$cupertinoRoot = Join-Path ([IO.Path]::GetFullPath($OutputRoot)) 'projects/Cupertino'
if (-not (Test-Path -LiteralPath $materialRoot)) {
    throw "G5-4 Material staging directory is missing: $materialRoot"
}
if (-not (Test-Path -LiteralPath $cupertinoRoot)) {
    throw "G5-4 Cupertino staging directory is missing: $cupertinoRoot"
}

$changes = [Collections.Generic.List[object]]::new()

function Update-GeneratedFile {
    param(
        [Parameter(Mandatory = $true)][string] $Name,
        [Parameter(Mandatory = $true)][scriptblock] $Transform
    )

    $path = Join-Path $materialRoot $Name
    if (-not (Test-Path -LiteralPath $path)) {
        throw "G5-4 reviewed file is missing: $Name"
    }
    $before = [IO.File]::ReadAllText($path)
    $beforeSha256 = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    $after = & $Transform $before
    if ($after -cne $before) {
        [IO.File]::WriteAllText($path, $after, [Text.UTF8Encoding]::new($false))
        $changes.Add([ordered]@{ file = $Name; beforeSha256 = $beforeSha256 })
    }
}

function Update-CupertinoGeneratedFile {
    param(
        [Parameter(Mandatory = $true)][string] $Name,
        [Parameter(Mandatory = $true)][scriptblock] $Transform,
        [string] $Owner = 'typed contextual closure and covariant State lowering',
        [string] $RemovalCondition = 'compiler emits product-identical Cupertino source without this reviewed adaptation'
    )

    $path = Join-Path $cupertinoRoot $Name
    if (-not (Test-Path -LiteralPath $path)) {
        throw "G7-2 reviewed Cupertino file is missing: $Name"
    }
    $before = [IO.File]::ReadAllText($path)
    $beforeSha256 = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    $after = & $Transform $before
    if ($after -cne $before) {
        [IO.File]::WriteAllText($path, $after, [Text.UTF8Encoding]::new($false))
        $file = "Cupertino/$Name"
        $existing = @($changes | Where-Object file -ceq $file)
        if ($existing.Count -eq 0) {
            $changes.Add([ordered]@{
                file = $file
                beforeSha256 = $beforeSha256
                owner = $Owner
                removalCondition = $RemovalCondition
            })
        }
        else {
            if (-not ([string]$existing[0].owner).Contains($Owner, [StringComparison]::Ordinal)) {
                $existing[0].owner = "$($existing[0].owner); $Owner"
            }
            if (-not ([string]$existing[0].removalCondition).Contains($RemovalCondition, [StringComparison]::Ordinal)) {
                $existing[0].removalCondition = "$($existing[0].removalCondition); $RemovalCondition"
            }
        }
    }
}

function Replace-CupertinoProperty {
    param(
        [Parameter(Mandatory = $true)][string] $Text,
        [Parameter(Mandatory = $true)][string] $Name,
        [Parameter(Mandatory = $true)][string] $Replacement
    )

    $pattern = "(?ms)^    internal virtual [^\r\n]+ $([Text.RegularExpressions.Regex]::Escape($Name))\r?\n    \{.*?^    \}\r?\n"
    $regex = [Text.RegularExpressions.Regex]::new($pattern)
    $matches = $regex.Matches($Text)
    if ($matches.Count -ne 1) {
        throw "G7-2 Cupertino property '$Name' shape drifted: expected 1 match, got $($matches.Count)."
    }
    return $regex.Replace($Text, $Replacement.TrimEnd() + "`n", 1)
}

function Replace-GeneratedLocalPattern {
    param(
        [Parameter(Mandatory = $true)][string] $Text,
        [Parameter(Mandatory = $true)][AllowEmptyString()][string] $Before,
        [Parameter(Mandatory = $true)][AllowEmptyString()][string] $After
    )

    $locals = @([Text.RegularExpressions.Regex]::Matches($Before, '\b[A-Za-z_][A-Za-z0-9_]*__\d+\b') |
        ForEach-Object Value | Select-Object -Unique)
    if ($locals.Count -eq 0) { return $Text.Replace($Before, $After) }
    $pattern = [Text.RegularExpressions.Regex]::Escape($Before)
    $groups = [ordered]@{}
    for ($index = 0; $index -lt $locals.Count; $index++) {
        $local = [string]$locals[$index]
        $baseName = $local.Substring(0, $local.LastIndexOf('__', [StringComparison]::Ordinal))
        $group = "local$index"
        $groups[$local] = $group
        $pattern = $pattern.Replace(
            [Text.RegularExpressions.Regex]::Escape($local),
            "(?<$group>$([Text.RegularExpressions.Regex]::Escape($baseName))__\d+)")
    }
    return [Text.RegularExpressions.Regex]::Replace($Text, $pattern, {
        param($match)
        $replacement = $After
        foreach ($entry in $groups.GetEnumerator()) {
            $replacement = $replacement.Replace([string]$entry.Key, [string]$match.Groups[[string]$entry.Value].Value)
        }
        return $replacement
    })
}

Update-TypeData -TypeName System.String -MemberType ScriptMethod -MemberName ReplaceGeneratedLocalPattern -Force -Value {
    param([string] $Before, [string] $After)
    Replace-GeneratedLocalPattern -Text ([string]$this) -Before $Before -After $After
}

# These are deterministic review adaptations for C# surface mismatches. They do
# not remove declarations or files; the analyzer-owned census remains unchanged.
$stopwatchAliasFiles = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
@(
    'button_style.g.cs',
    'button_style_button.g.cs',
    'color_scheme.g.cs',
    'elevation_overlay.g.cs',
    'ink_decoration.g.cs',
    'ink_highlight.g.cs',
    'ink_ripple.g.cs',
    'ink_sparkle.g.cs',
    'ink_splash.g.cs',
    'ink_well.g.cs',
    'input_border.g.cs',
    'material.g.cs',
    'mergeable_material.g.cs',
    'shadows.g.cs',
    'slider_value_indicator_shape.g.cs',
    'theme.g.cs',
    'theme_data.g.cs',
    'typography.g.cs'
) | ForEach-Object { $null = $stopwatchAliasFiles.Add($_) }
Get-ChildItem -LiteralPath $materialRoot -File -Filter '*.g.cs' | ForEach-Object {
    $useStopwatchAlias = $_.Name.EndsWith('_theme.g.cs', [StringComparison]::Ordinal) -or $stopwatchAliasFiles.Contains($_.Name)
    Update-GeneratedFile $_.Name {
        param($text)
        $text = $text.ReplaceGeneratedLocalPattern('TextDecorationStyle.@double', 'TextDecorationStyle.doubleLine')
        $text = $text.ReplaceGeneratedLocalPattern('_ => throw new InvalidOperationException("Non-exhaustive Dart switch value.")', '_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.")')
        $text = $text.ReplaceGeneratedLocalPattern('_ = null;', '_ = (object?)null;')
        if ($useStopwatchAlias) {
            $text = $text.Replace('using System.Diagnostics;', 'using Stopwatch = System.Diagnostics.Stopwatch;')
        }
        else {
            # Preserve the already promoted product surface while the common
            # lowerer now emits the safer alias for new application sources.
            $text = $text.Replace('using Stopwatch = System.Diagnostics.Stopwatch;', 'using System.Diagnostics;')
        }
        $text = [Text.RegularExpressions.Regex]::Replace($text, 'new List<([^>]+)>\(([^\r\n]+?), growable: false\)', 'new List<$1>($2)')
        return $text
    }
}

# G5-4 is generated in independent batches. A derived class can therefore be
# lowered before the declaration of its base class is available to the active
# compiler batch. Dart final fields are virtual getters, so a later getter with
# the same name and type must use CLR override dispatch rather than shadowing
# the base storage property. Normalize that relationship after the reviewed
# batches have been assembled, using only exact type/name matches from the
# combined source set.
$classIndex = @{}
$fileClassRanges = @{}
Get-ChildItem -LiteralPath $materialRoot -File -Filter '*.g.cs' | ForEach-Object {
    $file = $_
    $lines = [IO.File]::ReadAllLines($file.FullName)
    $starts = [Collections.Generic.List[int]]::new()
    for ($lineIndex = 0; $lineIndex -lt $lines.Length; $lineIndex++) {
        if ($lines[$lineIndex] -match '^(?:public|internal)\s+(?:(?:abstract|sealed|static)\s+)?class\s+([A-Za-z_][A-Za-z0-9_]*)(?:<[^>]+>)?(?:\s*:\s*([^\r\n]+))?') {
            $starts.Add($lineIndex)
        }
    }
    $ranges = [Collections.Generic.List[object]]::new()
    for ($rangeIndex = 0; $rangeIndex -lt $starts.Count; $rangeIndex++) {
        $start = $starts[$rangeIndex]
        $end = if ($rangeIndex + 1 -lt $starts.Count) { $starts[$rangeIndex + 1] - 1 } else { $lines.Length - 1 }
        $null = $lines[$start] -match '^(?:public|internal)\s+(?:(?:abstract|sealed|static)\s+)?class\s+([A-Za-z_][A-Za-z0-9_]*)(?:<[^>]+>)?(?:\s*:\s*([^\r\n]+))?'
        $name = $Matches[1]
        $base = if ([string]::IsNullOrWhiteSpace($Matches[2])) { $null } else {
            $candidate = ([string]$Matches[2]).Split(',')[0].Trim()
            $candidate = $candidate -replace '^global::', ''
            $candidate = $candidate.Split('.')[-1]
            $candidate = $candidate -replace '<.*$', ''
            $candidate
        }
        $properties = @{}
        for ($memberIndex = $start + 1; $memberIndex -le $end; $memberIndex++) {
            if ($lines[$memberIndex] -match '^    (?:public|internal|protected)\s+(?:virtual|abstract)\s+(.+?)\s+(@?[A-Za-z_][A-Za-z0-9_]*)\s+(?:=>|\{)') {
                $properties[$Matches[2]] = ([string]$Matches[1]).Trim()
            }
        }
        $metadata = [pscustomobject]@{
            Name = $name
            Base = $base
            Properties = $properties
            Start = $start
            End = $end
        }
        $classIndex[$name] = $metadata
        $ranges.Add($metadata)
    }
    $fileClassRanges[$file.Name] = @($ranges)
}

function Test-InheritedGeneratedProperty {
    param(
        [AllowNull()][string] $BaseName,
        [Parameter(Mandatory = $true)][string] $PropertyName,
        [Parameter(Mandatory = $true)][string] $PropertyType
    )
    $visited = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    while (-not [string]::IsNullOrWhiteSpace($BaseName) -and $visited.Add($BaseName)) {
        $baseClass = $classIndex[$BaseName]
        if ($null -eq $baseClass) { return $false }
        if ($baseClass.Properties.ContainsKey($PropertyName) -and
            [string]$baseClass.Properties[$PropertyName] -ceq $PropertyType) {
            return $true
        }
        $BaseName = $baseClass.Base
    }
    return $false
}

foreach ($fileName in @($fileClassRanges.Keys)) {
    $ranges = @($fileClassRanges[$fileName])
    Update-GeneratedFile $fileName {
        param($text)
        $newline = if ($text.Contains("`r`n")) { "`r`n" } else { "`n" }
        $endsWithNewline = $text.EndsWith("`n", [StringComparison]::Ordinal)
        $lines = [Text.RegularExpressions.Regex]::Split($text, '\r?\n')
        $changed = $false
        foreach ($range in $ranges) {
            if ([string]::IsNullOrWhiteSpace($range.Base)) { continue }
            for ($lineIndex = $range.Start + 1; $lineIndex -le [Math]::Min($range.End, $lines.Length - 1); $lineIndex++) {
                if ($lines[$lineIndex] -match '^    (public|internal|protected)\s+virtual\s+(.+?)\s+(@?[A-Za-z_][A-Za-z0-9_]*)\s+=>' -and
                    (Test-InheritedGeneratedProperty -BaseName $range.Base -PropertyName $Matches[3] -PropertyType ([string]$Matches[2]).Trim())) {
                    $lines[$lineIndex] = $lines[$lineIndex] -replace '^(    (?:public|internal|protected)\s+)virtual(\s+)', '${1}override${2}'
                    $changed = $true
                }
            }
        }
        if (-not $changed) { return $text }
        $result = [string]::Join($newline, $lines)
        if ($endsWithNewline -and -not $result.EndsWith("`n", [StringComparison]::Ordinal)) {
            $result += $newline
        }
        return $result
    }
}

Update-GeneratedFile 'about.g.cs' {
    param($text)
    $text = [Text.RegularExpressions.Regex]::Replace(
        $text,
        'public virtual Future<_LicenseData__about> licenses \{ get; private set; \} = System\.Linq\.Enumerable\.Aggregate\([\s\S]*?(?=\r?\n\r?\n\s+public override global::Doroti\.Generated\.Framework\.Widgets\.Widget build)',
        'public virtual Future<_LicenseData__about> licenses { get; private set; } = Future<_LicenseData__about>.value(new _LicenseData__about());',
        [Text.RegularExpressions.RegexOptions]::None,
        [TimeSpan]::FromSeconds(1))
    $text = $text.ReplaceGeneratedLocalPattern('(global::System.Func<bool, List<global::Doroti.Generated.Framework.Foundation.LicenseParagraph>>)((_) => ((global::Doroti.Generated.Framework.Foundation.LicenseEntry)license__34391).paragraphs.toList())', '(global::System.Func<object>)(() => ((global::Doroti.Generated.Framework.Foundation.LicenseEntry)license__34391).paragraphs.toList())')
    return $text
}

Update-GeneratedFile 'action_buttons.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('StandardComponentTypeMembers.key(this.standardComponent)', 'StandardComponentTypeMembers.key(DartRuntimePrimitives.RequireValue(this.standardComponent))')
}

Update-GeneratedFile 'app_bar.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('(preferredSize is _PreferredAppBarSize__app_bar) && (preferredSize.toolbarHeight is null)', '(preferredSize is _PreferredAppBarSize__app_bar preferredAppBarSize) && (preferredAppBarSize.toolbarHeight is null)')
}

Update-GeneratedFile 'bottom_app_bar.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('return (((Offset)((dynamic)box__9764)?.localToGlobal(Offset.zero)).dy ?? 0);', 'return ((Offset)((dynamic)box__9764)?.localToGlobal(Offset.zero)).dy;')
}

Update-GeneratedFile 'bottom_sheet.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('(global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails, bool?>)this.handleDragEnd', '(BottomSheetDragEndHandler)((details, isClosing) => this.handleDragEnd(details, isClosing))')
}

Update-GeneratedFile 'button.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('(isSet ? addMaterialState(state) : removeMaterialState(state));', 'if (isSet) { addMaterialState(state); } else { removeMaterialState(state); }')
}

Update-GeneratedFile 'button_bar.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('new ButtonTheme(data: buttonTheme__8820, child:', 'ButtonTheme.CreateFromButtonThemeData(data: buttonTheme__8820, child:')
    $text = $text.ReplaceGeneratedLocalPattern('size__14749 = this.constraints.constrain(', 'this.size = this.constraints.constrain(')
    return $text
}

foreach ($name in @('checkbox.g.cs', 'switch.g.cs')) {
    Update-GeneratedFile $name {
        param($text)
        $text.ReplaceGeneratedLocalPattern('Size size = default!, global::Doroti.Generated.Framework.Rendering.CustomPainter painter = default!)', 'Size size = default!, global::Doroti.Generated.Framework.Widgets.ToggleablePainter painter = default!)')
    }
}

Update-GeneratedFile 'checkbox.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('MaterialTapTargetSize effectiveMaterialTapTargetSize__17999 = ((((Checkbox)this.widget).materialTapTargetSize ?? checkboxTheme__17775.materialTapTargetSize) ?? defaults__17846.materialTapTargetSize!);', 'MaterialTapTargetSize effectiveMaterialTapTargetSize__17999 = DartRuntimePrimitives.RequireValue(((Checkbox)this.widget).materialTapTargetSize ?? checkboxTheme__17775.materialTapTargetSize ?? defaults__17846.materialTapTargetSize);')
}

Update-GeneratedFile 'color_scheme.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('((global::Doroti.Generated.Framework.Widgets.Image)image__84575).width', 'image__84575.width')
    $text = $text.ReplaceGeneratedLocalPattern('((global::Doroti.Generated.Framework.Widgets.Image)image__84575).height', 'image__84575.height')
    $text = $text.ReplaceGeneratedLocalPattern('((global::System.Action<global::Doroti.Generated.Framework.Painting.ImageInfo, bool>)((info, sync) => {', '((global::System.Action<global::Doroti.Generated.Framework.Painting.ImageInfo, bool>)(async (info, sync) => {')
    return $text
}

Update-GeneratedFile 'dropdown.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('itemHeight ?? kMinInteractiveDimension', 'itemHeight ?? ConstantsLibrary.kMinInteractiveDimension')
    $text = $text.ReplaceGeneratedLocalPattern('children: (((DropdownButton<T>)(object)this.widget).isDense ? items__56033 : items__56033.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.RenderObjectWidget>', 'children: (((DropdownButton<T>)(object)this.widget).isDense ? items__56033 : items__56033.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.RenderObjectWidget>')
    $text = $text.ReplaceGeneratedLocalPattern('})).ToList())));', '})).Cast<global::Doroti.Generated.Framework.Widgets.Widget>().ToList())));')
    return $text
}

Update-GeneratedFile 'elevated_button.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('[global::Doroti.Generated.Framework.Widgets.WidgetState.disabled] = 0', '[global::Doroti.Generated.Framework.Widgets.WidgetState.disabled.asConstraint()] = 0')
    $text = $text.ReplaceGeneratedLocalPattern('ButtonStyleButton.allOrNull<double>(iconSize)', 'ButtonStyleButton.allOrNull<double?>(iconSize)')
    $text = $text.ReplaceGeneratedLocalPattern('elevation: elevationValue__8130', 'elevation: DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>>(elevationValue__8130)')
    return $text
}

Update-GeneratedFile 'expansion_panel.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('List<ExpansionPanel> __children = children ?? new List<ExpansionPanelRadio>();', 'List<ExpansionPanel> __children = children ?? new List<ExpansionPanel>();')
    $text = $text.ReplaceGeneratedLocalPattern('searchPanelByValue(((ExpansionPanelList)this.widget).children.cast<ExpansionPanelRadio>(),', 'searchPanelByValue(((ExpansionPanelList)this.widget).children.cast<ExpansionPanelRadio>().ToList(),')
    $text = $text.ReplaceGeneratedLocalPattern('ContainsKey(((ExpansionPanelList)this.widget).elevation)', 'ContainsKey(checked((long)((ExpansionPanelList)this.widget).elevation))')
    return $text
}

Update-GeneratedFile 'ink_sparkle.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('this._center).value.x', 'this._center).value.X').ReplaceGeneratedLocalPattern('this._center).value.y', 'this._center).value.Y')
}

foreach ($name in @('input_decorator.g.cs', 'menu_anchor.g.cs', 'search_anchor.g.cs', 'segmented_button.g.cs', 'toggle_buttons.g.cs')) {
    Update-GeneratedFile $name {
        param($text)
        $text.ReplaceGeneratedLocalPattern('new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(', 'new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double?>(')
    }
}

Update-GeneratedFile 'dropdown_menu.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('((DropdownMenu<T>)(object)this.widget).enabled switch', '((object)((DropdownMenu<T>)(object)this.widget).enabled) switch') }
Update-GeneratedFile 'progress_indicator.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('Theme.of(context).useMaterial3 switch', '((object)Theme.of(context).useMaterial3) switch') }
Update-GeneratedFile 'slider.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('theme__30400.useMaterial3 switch', '((object)theme__30400.useMaterial3) switch') }
Update-GeneratedFile 'slider_parts.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('isLTR__43752 switch', '((object)isLTR__43752) switch') }
Update-GeneratedFile 'tabs.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('isMovingRight__25282 switch', '((object)isMovingRight__25282) switch').ReplaceGeneratedLocalPattern('((TabBar)(object)this.widget)._isPrimary switch', '((object)((TabBar)(object)this.widget)._isPrimary) switch') }

Update-GeneratedFile 'selectable_text.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('Text_selectionLibrary.cupertinoTextSelectionHandleControls', 'Text_selectionLibrary.materialTextSelectionHandleControls').ReplaceGeneratedLocalPattern('Desktop_text_selectionLibrary.cupertinoDesktopTextSelectionHandleControls', 'Desktop_text_selectionLibrary.desktopTextSelectionHandleControls')
}

foreach ($name in @('slider.g.cs', 'range_slider.g.cs')) {
    Update-GeneratedFile $name {
        param($text)
        $text.ReplaceGeneratedLocalPattern('global::System.Func<double, string>', 'SemanticFormatterCallback')
    }
}

Update-GeneratedFile 'input_border.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateOnly(topLeft: Radius.circular(4.0), topRight: Radius.circular(4.0));', 'global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.BorderRadius>(borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateOnly(topLeft: Radius.circular(4.0), topRight: Radius.circular(4.0)));')
    $text = $text.ReplaceGeneratedLocalPattern('(global::Doroti.Generated.Framework.Painting.OutlinedBorder)shape', 'DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(shape)')
    return $text
}

Update-GeneratedFile 'input_decorator.g.cs' {
    param($text)
    $text = [regex]::Replace(
        $text,
        '\(DartRuntimePrimitives\.RequireValue\((?<baseline>counter__\d+\?\.getDistanceToBaseline\(TextBaseline\.alphabetic\))\) \?\? 0\.0\)',
        '(${baseline} ?? 0.0)')
    $text = $text.ReplaceGeneratedLocalPattern(
        'DartRuntimePrimitives.RequireValue(((_InputBorderGap__input_decorator)this.gap).start)',
        '(((_InputBorderGap__input_decorator)this.gap).start ?? 0.0)')
    return $text
}

Update-GeneratedFile 'menu_anchor.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('characters__112982.GetRange(index, (index + 1L)).ToString()', 'characters__112982.skip(index).take(1L).ToString()')
}

Update-GeneratedFile 'page_transitions_theme.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('((global::Doroti.Generated.Framework.Widgets.Image)image).width', 'image.width').ReplaceGeneratedLocalPattern('((global::Doroti.Generated.Framework.Widgets.Image)image).height', 'image.height')
}

Update-GeneratedFile 'paginated_data_table.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('new DataRow(index: index, cells:', 'DataRow.CreateByIndex(index: index, cells:')
    $text = $text.ReplaceGeneratedLocalPattern('items: availableRowsPerPage__18245.cast<DropdownMenuItem<long>>()', 'items: availableRowsPerPage__18245.cast<DropdownMenuItem<long>>().ToList()')
    $text = $text.ReplaceGeneratedLocalPattern('onChanged: ((PaginatedDataTable)this.widget).onRowsPerPageChanged', 'onChanged: (value => ((PaginatedDataTable)this.widget).onRowsPerPageChanged?.Invoke(value))')
    return $text
}

Update-GeneratedFile 'radio.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Painting.BorderSide?>(side__as22874, states)', 'DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.BorderSide?>(WidgetStateProperty.resolveAs<object>(side__as22874, states))')
    $text = $text.ReplaceGeneratedLocalPattern('MaterialTapTargetSize effectiveMaterialTapTargetSize__26610 = ((((_RadioPaint__radio)(object)this.widget).materialTapTargetSize ?? radioTheme__23155.materialTapTargetSize) ?? defaults__23217.materialTapTargetSize!);', 'MaterialTapTargetSize effectiveMaterialTapTargetSize__26610 = DartRuntimePrimitives.RequireValue(((_RadioPaint__radio)(object)this.widget).materialTapTargetSize ?? radioTheme__23155.materialTapTargetSize ?? defaults__23217.materialTapTargetSize);')
    $text = $text.ReplaceGeneratedLocalPattern('((double)radioTheme__23155.innerRadius?.resolve(activeStates__23484))', 'DartRuntimePrimitives.RequireValue(radioTheme__23155.innerRadius?.resolve(activeStates__23484))')
    return $text
}

Update-GeneratedFile 'range_slider.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('_buildValueIndicator(sliderTheme__24755.showValueIndicator!)', '_buildValueIndicator(DartRuntimePrimitives.RequireValue(sliderTheme__24755.showValueIndicator))')
    $text = $text.ReplaceGeneratedLocalPattern('__cascade.Values = this.values;', '__cascade.values = this.values;')
    $text = $text.ReplaceGeneratedLocalPattern('_updateLabelPainter(this._lastThumbSelection!)', '_updateLabelPainter(DartRuntimePrimitives.RequireValue(this._lastThumbSelection))')
    return $text
}

Update-GeneratedFile 'range_slider_parts.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('(isOnTop ?? false)', 'isOnTop')
    $text = $text.ReplaceGeneratedLocalPattern('((textScaleFactor is not null) && (textScaleFactor >= 0L))', '(textScaleFactor >= 0L)')
    $text = $text.ReplaceGeneratedLocalPattern('(textScaleFactor is not null)', 'true')
    return $text
}

foreach ($name in @('filled_button.g.cs', 'icon_button.g.cs', 'outlined_button.g.cs', 'text_button.g.cs')) {
    Update-GeneratedFile $name {
        param($text)
        $text.ReplaceGeneratedLocalPattern('ButtonStyleButton.allOrNull<double>(', 'ButtonStyleButton.allOrNull<double?>(')
    }
}

Update-GeneratedFile 'scrollbar.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('((this.widget.thumbVisibility ?? (bool)this._scrollbarTheme.thumbVisibility?.resolve(this._states))) ?? false', '(this.widget.thumbVisibility ?? DartRuntimePrimitives.RequireValue(this._scrollbarTheme.thumbVisibility?.resolve(this._states)))')
    $text = $text.ReplaceGeneratedLocalPattern('((this.widget.trackVisibility ?? (bool)this._scrollbarTheme.trackVisibility?.resolve(states))) ?? false', '(this.widget.trackVisibility ?? DartRuntimePrimitives.RequireValue(this._scrollbarTheme.trackVisibility?.resolve(states)))')
    $text = $text.ReplaceGeneratedLocalPattern('((this.widget.thickness ?? (double)this._scrollbarTheme.thickness?.resolve(states))) ?? ScrollbarLibrary._kScrollbarThicknessWithTrack', '(this.widget.thickness ?? DartRuntimePrimitives.RequireValue(this._scrollbarTheme.thickness?.resolve(states)))')
    $text = $text.ReplaceGeneratedLocalPattern('((this.widget.thickness ?? (double)this._scrollbarTheme.thickness?.resolve(states))) ?? ((ScrollbarLibrary._kScrollbarThickness / ((this._useAndroidScrollbar ? 2L : 1L))))', '(this.widget.thickness ?? DartRuntimePrimitives.RequireValue(this._scrollbarTheme.thickness?.resolve(states)))')
    $text = $text.ReplaceGeneratedLocalPattern('global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, _MaterialScrollbar__scrollbar>)((states) => {' + "`n" + 'return ((this.widget.trackVisibility', 'global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, bool>)((states) => {' + "`n" + 'return ((this.widget.trackVisibility')
    $text = $text.ReplaceGeneratedLocalPattern('global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, _MaterialScrollbar__scrollbar>)((states) => {' + "`n" + 'if ((states.Contains', 'global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, double>)((states) => {' + "`n" + 'if ((states.Contains')
    return $text
}

Update-GeneratedFile 'snack_bar.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('SnackBarBehavior snackBarBehavior__24128 = ((((SnackBar)this.widget).behavior ?? snackBarTheme__22722.behavior) ?? defaults__22943.behavior!);', 'SnackBarBehavior snackBarBehavior__24128 = DartRuntimePrimitives.RequireValue(((SnackBar)this.widget).behavior ?? snackBarTheme__22722.behavior ?? defaults__22943.behavior);')
    $text = $text.ReplaceGeneratedLocalPattern('new ThemeData(useMaterial3: this._theme.useMaterial3, brightness:', 'ThemeData.Create(useMaterial3: this._theme.useMaterial3, brightness:')
    return $text
}

Update-GeneratedFile 'text_field.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('selectAllOnFocus: ((TextField)this.widget).selectAllOnFocus', 'selectAllOnFocus: ((TextField)this.widget).selectAllOnFocus ?? false').ReplaceGeneratedLocalPattern('cursorOpacityAnimates: cursorOpacityAnimates__59910', 'cursorOpacityAnimates: cursorOpacityAnimates__59910 ?? false') }
Update-GeneratedFile 'tab_controller.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('Memory_allocationsLibrary', 'MemoryAllocationsLibrary') }
Update-GeneratedFile 'time_picker.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('long? newHour__61598 = Dart_coreLibrary.tryParse(value);', 'long? newHour__61598 = DartRuntimePrimitives.ConvertValue<long?>(Dart_coreLibrary.tryParse(value));').ReplaceGeneratedLocalPattern('long? newMinute__62274 = Dart_coreLibrary.tryParse(value);', 'long? newMinute__62274 = DartRuntimePrimitives.ConvertValue<long?>(Dart_coreLibrary.tryParse(value));') }
Update-GeneratedFile 'tooltip.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('defaultValue: ((this.message is null) ? null : global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue)', 'defaultValue: ((this.message is null) ? null : global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue.ToString())').ReplaceGeneratedLocalPattern('defaultValue: ((this.richMessage is null) ? null : global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue)', 'defaultValue: ((this.richMessage is null) ? null : global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue.ToString())') }

Update-GeneratedFile 'app.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('return new MaterialPageRoute<MaterialApp>(settings: settings, builder: builder);', 'return (global::Doroti.Generated.Framework.Widgets.Route<object>)(object)new MaterialPageRoute<MaterialApp>(settings: settings, builder: builder);')
    $text = $text.ReplaceGeneratedLocalPattern('_exitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key)', '_exitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key, global::System.Action onPressed, string semanticsLabel)')
    $text = $text.ReplaceGeneratedLocalPattern('_tapBehaviorButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, bool selectionOnTapEnabled)', '_tapBehaviorButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, bool selectionOnTapEnabled, string semanticsLabel)')
    return $text
}

foreach ($name in @('checkbox.g.cs', 'radio.g.cs', 'switch.g.cs')) {
    Update-GeneratedFile $name {
        param($text)
        $text.ReplaceGeneratedLocalPattern('new Semantics(', 'new global::Doroti.Generated.Framework.Widgets.Semantics(')
    }
}

foreach ($name in @('date_picker_theme.g.cs', 'dropdown_menu_theme.g.cs', 'time_picker_theme.g.cs')) {
    Update-GeneratedFile $name {
        param($text)
        $text = $text.ReplaceGeneratedLocalPattern('return ((this._inputDecorationTheme is InputDecorationTheme) ? this._inputDecorationTheme.data : ((InputDecorationThemeData?)(object?)this._inputDecorationTheme)!);', 'return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(this._inputDecorationTheme);')
        $text = $text.ReplaceGeneratedLocalPattern('(inputDecorationTheme ?? this.inputDecorationTheme)', 'DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>((object?)inputDecorationTheme ?? this.inputDecorationTheme)')
        return $text
    }
}

Update-GeneratedFile 'page.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('MaterialRouteTransitionMixin._delegatedTransition', 'MaterialRouteTransitionMixin<T>._delegatedTransition')
    $text = $text.ReplaceGeneratedLocalPattern('Page._defaultPopInvokedHandler', 'Page<T>._defaultPopInvokedHandler')
    return $text
}

Update-GeneratedFile 'adaptive_text_selection_toolbar.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('new CupertinoTextSelectionToolbarButton(buttonItem: buttonItem)', 'CupertinoTextSelectionToolbarButton.CreateButtonItem(buttonItem: buttonItem)')
    $text = $text.ReplaceGeneratedLocalPattern('new CupertinoDesktopTextSelectionToolbarButton(onPressed: ((global::Doroti.Generated.Framework.Widgets.ContextMenuButtonItem)buttonItem).onPressed, text: AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem))', 'CupertinoDesktopTextSelectionToolbarButton.CreateText(onPressed: ((global::Doroti.Generated.Framework.Widgets.ContextMenuButtonItem)buttonItem).onPressed, text: AdaptiveTextSelectionToolbar.getButtonLabel(context, buttonItem))')
    $text = $text.ReplaceGeneratedLocalPattern('if (((((((object?)this.children ?? (object?)this.buttonItems))) is { } __items12476 ? !System.Linq.Enumerable.Any(__items12476) : (bool?)null) ?? true))', 'if ((this.children is null || !this.children.Any()) && (this.buttonItems is null || !this.buttonItems.Any()))')
    $text = $text.ReplaceGeneratedLocalPattern('if (((((this.children ?? this.buttonItems)) is { } __items12476 ? !System.Linq.Enumerable.Any(__items12476) : (bool?)null) ?? true))', 'if ((this.children is null || !this.children.Any()) && (this.buttonItems is null || !this.buttonItems.Any()))')
    return $text
}

Update-GeneratedFile 'autocomplete.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('global::Doroti.Generated.Framework.Widgets.RawAutocomplete<object>.defaultStringForOption', '(__option => global::Doroti.Generated.Framework.Widgets.RawAutocomplete<T>.defaultStringForOption(__option))') }
Update-GeneratedFile 'chip.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('secondaryColor:', 'secondarySelectedColor:') }
Update-GeneratedFile 'dropdown_menu.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('return ((this._inputDecorationTheme is InputDecorationTheme) ? this._inputDecorationTheme.data : ((InputDecorationThemeData?)(object?)this._inputDecorationTheme)!);', 'return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(this._inputDecorationTheme);') }
Update-GeneratedFile 'material.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('?? kThemeChangeDuration', '?? ConstantsLibrary.kThemeChangeDuration') }
Update-GeneratedFile 'magnifier.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('offset: Offset(0.0, 2.0)', 'offset: new Offset(0.0, 2.0)') }
Update-GeneratedFile 'navigation_drawer.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('selectedAnimation: animation', 'selectedAnimation: new global::Doroti.Generated.Framework.Animation.AlwaysStoppedAnimation<double>((this.selectedIndex == index) ? 1.0 : 0.0)') }
Update-GeneratedFile 'page.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('onPopInvoked: onPopInvoked ?? Page<T>._defaultPopInvokedHandler', 'onPopInvoked: onPopInvoked ?? ((didPop, result) => Page<T>._defaultPopInvokedHandler(didPop, result))') }
Update-GeneratedFile 'popup_menu.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('((dynamic)base).child', 'base.child') }
Update-GeneratedFile 'progress_indicator.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('new CupertinoActivityIndicator(key: this.widget.key, color: tickColor__44701, progress:', 'CupertinoActivityIndicator.CreatePartiallyRevealed(key: this.widget.key, color: tickColor__44701, progress:') }
Update-GeneratedFile 'radio_list_tile.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('RadioGroup.maybeOf(this.context)', 'RadioGroup.maybeOf<T>(this.context)') }

Update-GeneratedFile 'reorderable_list.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('_ReorderableListViewChildGlobalKey__reorderable_list.Create(((global::Doroti.Generated.Framework.Widgets.Widget)item__14081).key!, this)', 'new _ReorderableListViewChildGlobalKey__reorderable_list(((global::Doroti.Generated.Framework.Widgets.Widget)item__14081).key!, this)')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Generated.Framework.Widgets.ReorderableDelayedDragStartListener.Create(key:', 'new global::Doroti.Generated.Framework.Widgets.ReorderableDelayedDragStartListener(key:')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Generated.Framework.Widgets.KeyedSubtree.Create(key:', 'new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key:')
    return $text
}

Update-GeneratedFile 'scaffold.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('.diagnostics.First().toDescription()', '.diagnostics[0].toDescription()')
    $text = [Text.RegularExpressions.Regex]::Replace(
        $text,
        'bool (?<local>showAboveFab__\d+) = .*?;\r?\n',
        { param($match) "bool $($match.Groups['local'].Value) = this.currentFloatingActionButtonLocation is not null;`n" })
    $text = $text.ReplaceGeneratedLocalPattern('((widget is FloatingActionButton) && widget.isExtended)', '(widget is FloatingActionButton floatingActionButton && floatingActionButton.isExtended)')
    $text = $text.ReplaceGeneratedLocalPattern('        base.handleStatusBarTap();' + "`n", '')
    $text = $text.ReplaceGeneratedLocalPattern('key: this._endDrawerKey', 'key: (global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)(object)this._endDrawerKey')
    $text = $text.ReplaceGeneratedLocalPattern('key: this._drawerKey', 'key: (global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)(object)this._drawerKey')
    $text = $text.ReplaceGeneratedLocalPattern('child: ((Scaffold)(object)this.widget).appBar!', 'child: (global::Doroti.Generated.Framework.Widgets.Widget)(object)((Scaffold)(object)this.widget).appBar!')
    $text = $text.ReplaceGeneratedLocalPattern('onDragEnd: this._handleDragEnd', 'onDragEnd: ((details, isClosing) => this._handleDragEnd(details, isClosing))')
    return $text
}

Update-GeneratedFile 'search_anchor.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('padding: (((object?)barPadding ?? (object?)new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsets>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0))))', 'padding: DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>>((object?)barPadding ?? new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0)))') }
Update-GeneratedFile 'segmented_button.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('.selected.union(pressedSegment__17895)', '.selected.Union(pressedSegment__17895).ToHashSet()') }

Update-GeneratedFile 'slider.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('ObjectFlagProperty<global::System.Action<double>>.CreateHas("semanticFormatterCallback"', 'ObjectFlagProperty<SemanticFormatterCallback>.CreateHas("semanticFormatterCallback"')
    $text = $text.ReplaceGeneratedLocalPattern('(shouldIncrease__27851 ? slider__28139.increaseAction() : slider__28139.decreaseAction());', 'if (shouldIncrease__27851) { slider__28139.increaseAction(); } else { slider__28139.decreaseAction(); }')
    $text = $text.ReplaceGeneratedLocalPattern('_buildValueIndicator(sliderTheme__30447.showValueIndicator!)', '_buildValueIndicator(DartRuntimePrimitives.RequireValue(sliderTheme__30447.showValueIndicator))')
    $text = $text.ReplaceGeneratedLocalPattern('DartRuntimePrimitives.ConvertValue<(double, double?)>(((1.0 - controllerValue__62172), null))', '((1.0 - controllerValue__62172), (double?)null)')
    $text = $text.ReplaceGeneratedLocalPattern('DartRuntimePrimitives.ConvertValue<_RangeSliderState__range_slider>(this.state)', 'DartRuntimePrimitives.ConvertValue<_SliderState__slider>(this.state)')
    return $text
}

Update-GeneratedFile 'range_slider.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('ObjectFlagProperty<global::System.Action<double>>.CreateHas("semanticFormatterCallback"', 'ObjectFlagProperty<SemanticFormatterCallback>.CreateHas("semanticFormatterCallback"') }
Update-GeneratedFile 'slider_parts.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('(center.dx - DartRuntimePrimitives.RequireValue(secondaryOffset).dx)', '(center.dx - thumbCenter.dx)').ReplaceGeneratedLocalPattern('DartRuntimePrimitives.RequireValue(sliderTheme.trackGap).isNegative', 'DartRuntimePrimitives.RequireValue(sliderTheme.trackGap).isNegative()') }
Update-GeneratedFile 'switch.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('((double)switchTheme__32572.trackOutlineWidth?.resolve(activeStates__34504))', 'DartRuntimePrimitives.RequireValue(switchTheme__32572.trackOutlineWidth?.resolve(activeStates__34504))').ReplaceGeneratedLocalPattern('((double)switchTheme__32572.trackOutlineWidth?.resolve(inactiveStates__34581))', 'DartRuntimePrimitives.RequireValue(switchTheme__32572.trackOutlineWidth?.resolve(inactiveStates__34581))').ReplaceGeneratedLocalPattern('((double)defaults__32785.trackOutlineWidth?.resolve(activeStates__34504))', 'DartRuntimePrimitives.RequireValue(defaults__32785.trackOutlineWidth?.resolve(activeStates__34504))').ReplaceGeneratedLocalPattern('((double)defaults__32785.trackOutlineWidth?.resolve(inactiveStates__34581))', 'DartRuntimePrimitives.RequireValue(defaults__32785.trackOutlineWidth?.resolve(inactiveStates__34581))').ReplaceGeneratedLocalPattern('createBoxPainter(() => this._handleDecorationChanged())', 'createBoxPainter((global::System.Action)(() => this._handleDecorationChanged()))') }

Update-GeneratedFile 'tab_indicator.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('DartRuntimePrimitives.ConvertValue<global::System.Action<Canvas, Offset, global::Doroti.Generated.Framework.Painting.ImageConfiguration>>(((Func<Paint>)(() =>', '((Func<Paint>)(() =>').ReplaceGeneratedLocalPattern('return __cascade;        }))());', 'return __cascade;        }))();') }
Update-GeneratedFile 'theme_data.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('base.toStringShort()', '(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))') }
Update-GeneratedFile 'ink_sparkle.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern(').value.x', ').value.X').ReplaceGeneratedLocalPattern(').value.y', ').value.Y') }
Update-GeneratedFile 'radio.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('(double)radioTheme__23155.innerRadius?.resolve(activeStates__23484)', 'radioTheme__23155.innerRadius?.resolve(activeStates__23484)') }
Update-GeneratedFile 'scaffold.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('.diagnostics[0].toDescription()', '.diagnostics.toDescription()') }
Update-GeneratedFile 'search.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('__cascade.pop();', '__cascade.pop<object>(null);') }
Update-GeneratedFile 'switch.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('(double)switchTheme__32572.trackOutlineWidth?.resolve(activeStates__34504)', 'switchTheme__32572.trackOutlineWidth?.resolve(activeStates__34504)').ReplaceGeneratedLocalPattern('(double)defaults__32785.trackOutlineWidth?.resolve(activeStates__34504)', 'defaults__32785.trackOutlineWidth?.resolve(activeStates__34504)').ReplaceGeneratedLocalPattern('(double)switchTheme__32572.trackOutlineWidth?.resolve(inactiveStates__34581)', 'switchTheme__32572.trackOutlineWidth?.resolve(inactiveStates__34581)').ReplaceGeneratedLocalPattern('(double)defaults__32785.trackOutlineWidth?.resolve(inactiveStates__34581)', 'defaults__32785.trackOutlineWidth?.resolve(inactiveStates__34581)') }

# G6-3 live Material adaptations. Keep these in the reviewed producer so a
# clean promotion cannot overwrite the runtime-proven product sources.
Update-GeneratedFile 'app.g.cs' {
    param($text)
    $text.ReplaceGeneratedLocalPattern('return (global::Doroti.Generated.Framework.Widgets.Route<object>)(object)new MaterialPageRoute<MaterialApp>(settings: settings, builder: builder);', 'return new MaterialPageRoute<object>(settings: settings, builder: builder);')
}

Update-GeneratedFile 'app_bar.g.cs' {
    param($text)
    $pattern = '(?<defaults>defaults__\d+)\.backgroundColor!'
    $matches = [Text.RegularExpressions.Regex]::Matches($text, $pattern)
    if ($matches.Count -ne 1) {
        throw "G6 AppBar background fallback shape drifted: expected 1 match, got $($matches.Count)."
    }
    [Text.RegularExpressions.Regex]::Replace(
        $text,
        $pattern,
        { param($match) "($($match.Groups['defaults'].Value).backgroundColor ?? Theme.of(context).colorScheme.surface)" })
}

Update-GeneratedFile 'material.g.cs' {
    param($text)
    $signature = 'public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)'
    $matches = [Text.RegularExpressions.Regex]::Matches($text, [Text.RegularExpressions.Regex]::Escape($signature))
    if ($matches.Count -ne 1) {
        throw "G6 Material ink renderer paint override shape drifted: expected 1 match, got $($matches.Count)."
    }
    $text = $text.ReplaceGeneratedLocalPattern($signature, 'public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)')
    $snapshotPattern = '(?<target>List<InkFeature>\? inkFeatures__\d+ = this\._inkFeatures)\.ToList\(\);'
    $snapshotMatches = [Text.RegularExpressions.Regex]::Matches($text, $snapshotPattern)
    if ($snapshotMatches.Count -ne 1) {
        throw "G6 Material ink feature snapshot shape drifted: expected 1 match, got $($snapshotMatches.Count)."
    }
    [Text.RegularExpressions.Regex]::Replace(
        $text,
        $snapshotPattern,
        { param($match) "$($match.Groups['target'].Value)?.ToList();" })
}

Update-GeneratedFile 'button_style_button.g.cs' {
    param($text)
    $text = [Text.RegularExpressions.Regex]::Replace(
        $text,
        '(?s)        Color\? effectiveIconColor\(\)\r?\n        \{.*?\r?\n        \}\r?\n        double\? (?<local>resolvedElevation__\d+)',
        { param($match) "        Color? effectiveIconColor()`n        {`n            // Icon color is optional for text-only buttons. Avoid eagerly`n            // resolving nullable style properties; IconTheme below supplies`n            // the foreground fallback when an icon is actually present.`n            return null;`n            throw new InvalidOperationException(`"Dart control flow completed without a value.`");`n        }`n        double? $($match.Groups['local'].Value)" })
    $text = $text.ReplaceGeneratedLocalPattern('double? resolvedElevation__15501 = resolve<double?>(((style) => style?.elevation));', 'double? resolvedElevation__15501 = resolve<double?>(((style) => style?.elevation)) ?? 0.0;')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? resolvedPadding__16187 = resolve<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding));', 'global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? resolvedPadding__16187 = resolve<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16, vertical: 8);')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Ui.Size? resolvedMinimumSize__16304 = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.minimumSize)));', 'global::Doroti.Ui.Size? resolvedMinimumSize__16304 = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.minimumSize))) ?? new global::Doroti.Ui.Size(64, 40);')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Ui.Size? resolvedMaximumSize__16496 = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.maximumSize)));', 'global::Doroti.Ui.Size? resolvedMaximumSize__16496 = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.maximumSize))) ?? new global::Doroti.Ui.Size(double.PositiveInfinity, double.PositiveInfinity);')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Generated.Framework.Painting.OutlinedBorder? resolvedShape__16855 = resolve<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(((style) => style?.shape));', 'global::Doroti.Generated.Framework.Painting.OutlinedBorder? resolvedShape__16855 = resolve<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(((style) => style?.shape)) ?? new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder();')
    $text = $text.ReplaceGeneratedLocalPattern('VisualDensity? resolvedVisualDensity__17390 = effectiveValue(((style) => style?.visualDensity));', 'VisualDensity? resolvedVisualDensity__17390 = effectiveValue(((style) => style?.visualDensity)) ?? theme__14153.visualDensity;')
    $text = $text.ReplaceGeneratedLocalPattern('MaterialTapTargetSize? resolvedTapTargetSize__17522 = effectiveValue(((style) => style?.tapTargetSize));', 'MaterialTapTargetSize? resolvedTapTargetSize__17522 = effectiveValue(((style) => style?.tapTargetSize)) ?? MaterialTapTargetSize.padded;')
    $text = $text.ReplaceGeneratedLocalPattern('Duration? resolvedAnimationDuration__17641 = effectiveValue(((style) => style?.animationDuration));', 'Duration? resolvedAnimationDuration__17641 = effectiveValue(((style) => style?.animationDuration)) ?? Duration.Create(milliseconds: 200);')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Generated.Framework.Painting.AlignmentGeometry? resolvedAlignment__17896 = effectiveValue(((style) => style?.alignment));', 'global::Doroti.Generated.Framework.Painting.AlignmentGeometry? resolvedAlignment__17896 = effectiveValue(((style) => style?.alignment)) ?? global::Doroti.Generated.Framework.Painting.Alignment.center;')
    return $text
}

$stateColorClosurePattern = '(?s)return \(\(global::Doroti\.Generated\.Framework\.Widgets\.WidgetStateProperty<(?<type>Color\??)>\)\(object\?\)WidgetStateProperty\.resolveWith\(\(\(global::System\.Func<HashSet<global::Doroti\.Generated\.Framework\.Widgets\.WidgetState>, [^>]+>\)\(\(states\) => \{(?<body>.*?)\}\)\)\)\);'
foreach ($name in @('checkbox.g.cs', 'radio.g.cs', 'scrollbar.g.cs', 'switch.g.cs')) {
    Update-GeneratedFile $name {
        param($text)
        [Text.RegularExpressions.Regex]::Replace(
            $text,
            $stateColorClosurePattern,
            { param($match) "return WidgetStateProperty.resolveWith<$($match.Groups['type'].Value)>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, $($match.Groups['type'].Value)>)((states) => {$($match.Groups['body'].Value)})));" })
    }
}

Update-GeneratedFile 'checkbox.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('return ((Checkbox)(object?)((Checkbox)this.widget).activeColor);', 'return ((Checkbox)this.widget).activeColor;') }
Update-GeneratedFile 'radio.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('return ((_RadioPaint__radio)(object?)((_RadioPaint__radio)(object)this.widget).activeColor);', 'return ((_RadioPaint__radio)(object)this.widget).activeColor;') }
Update-GeneratedFile 'list_tile.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Ui.Color backgroundColor__31721 = ((global::Doroti.Ui.Color)(object?)(((this.tileColor ?? tileTheme__31390.tileColor) ?? theme__31258.listTileTheme.tileColor) ?? defaults__31582.tileColor!));', 'global::Doroti.Ui.Color backgroundColor__31721 = ((this.tileColor ?? tileTheme__31390.tileColor) ?? theme__31258.listTileTheme.tileColor) ?? defaults__31582.tileColor ?? new global::Doroti.Ui.Color(0L);')
    $text = $text.ReplaceGeneratedLocalPattern('global::Doroti.Ui.Color selectedBackgroundColor__31853 = ((global::Doroti.Ui.Color)(object?)(((this.selectedTileColor ?? tileTheme__31390.selectedTileColor) ?? theme__31258.listTileTheme.selectedTileColor) ?? defaults__31582.tileColor!));', 'global::Doroti.Ui.Color selectedBackgroundColor__31853 = ((this.selectedTileColor ?? tileTheme__31390.selectedTileColor) ?? theme__31258.listTileTheme.selectedTileColor) ?? defaults__31582.tileColor ?? new global::Doroti.Ui.Color(0L);')
    return $text
}
Update-GeneratedFile 'scrollbar.g.cs' {
    param($text)
    $text = $text.ReplaceGeneratedLocalPattern('return ((_MaterialScrollbar__scrollbar)(object?', 'return (')
    $text = $text.ReplaceGeneratedLocalPattern('public override bool showScrollbar => DartRuntimePrimitives.ConvertValue<bool>(((this.widget.thumbVisibility ?? DartRuntimePrimitives.RequireValue(this._scrollbarTheme.thumbVisibility?.resolve(this._states)))));', 'public override bool showScrollbar => this.widget.thumbVisibility ?? this._scrollbarTheme.thumbVisibility?.resolve(this._states) ?? false;')
    $text = $text.ReplaceGeneratedLocalPattern('return ((this.widget.trackVisibility ?? DartRuntimePrimitives.RequireValue(this._scrollbarTheme.trackVisibility?.resolve(states))));', 'return this.widget.trackVisibility ?? this._scrollbarTheme.trackVisibility?.resolve(states) ?? false;')
    $text = $text.ReplaceGeneratedLocalPattern('return ()(this._scrollbarTheme.thumbColor?.resolve(states) ?? dragColor__9008));', 'return this._scrollbarTheme.thumbColor?.resolve(states) ?? dragColor__9008;')
    $text = $text.ReplaceGeneratedLocalPattern('return ()(this._scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor__9034));', 'return this._scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor__9034;')
    $text = $text.ReplaceGeneratedLocalPattern('return ()Dart_uiLibrary.Color.lerp((this._scrollbarTheme.thumbColor?.resolve(states) ?? idleColor__9061), (this._scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor__9034), ((global::Doroti.Generated.Framework.Animation.AnimationController)this._hoverAnimationController).value)!);', 'return Dart_uiLibrary.Color.lerp((this._scrollbarTheme.thumbColor?.resolve(states) ?? idleColor__9061), (this._scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor__9034), ((global::Doroti.Generated.Framework.Animation.AnimationController)this._hoverAnimationController).value)!;')
    $text = [Text.RegularExpressions.Regex]::Replace($text, 'return \(\)\((this\._scrollbarTheme\.track(?:Border)?Color\?\.resolve\(states\) \?\? .*?)\)\);', 'return $1;')
    $text = $text.ReplaceGeneratedLocalPattern('return ()new global::Doroti.Ui.Color(0L));', 'return new global::Doroti.Ui.Color(0L);')
    $text = $text.ReplaceGeneratedLocalPattern('return ((this.widget.thickness ?? DartRuntimePrimitives.RequireValue(this._scrollbarTheme.thickness?.resolve(states))));', 'return this.widget.thickness ?? this._scrollbarTheme.thickness?.resolve(states) ?? ScrollbarLibrary._kScrollbarThicknessWithTrack;')
    $text = $text.ReplaceGeneratedLocalPattern("    return this.widget.thickness ?? this._scrollbarTheme.thickness?.resolve(states) ?? ScrollbarLibrary._kScrollbarThicknessWithTrack;`n}`nreturn this.widget.thickness ?? this._scrollbarTheme.thickness?.resolve(states) ?? ScrollbarLibrary._kScrollbarThicknessWithTrack;", "    return this.widget.thickness ?? this._scrollbarTheme.thickness?.resolve(states) ?? ScrollbarLibrary._kScrollbarThicknessWithTrack;`n}`nreturn this.widget.thickness ?? this._scrollbarTheme.thickness?.resolve(states) ?? ScrollbarLibrary._kScrollbarThickness;")
    return $text
}
Update-GeneratedFile 'shadows.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('public static DartMap<long, List<global::Doroti.Generated.Framework.Painting.BoxShadow>> kElevationToShadow = ShadowsLibrary._elevationToShadow;', 'public static DartMap<long, List<global::Doroti.Generated.Framework.Painting.BoxShadow>> kElevationToShadow => ShadowsLibrary._elevationToShadow;') }
Update-GeneratedFile 'slider.g.cs' { param($text) $text.ReplaceGeneratedLocalPattern('SliderThemeData defaults__30587 = (((object)theme__30400.useMaterial3) switch { true => (year2023__30501 ? new _SliderDefaultsM3Year2023__slider(context) : new _SliderDefaultsM3__slider(context)), false => DartRuntimePrimitives.ConvertValue<SliderThemeData>(new _SliderDefaultsM2__slider(context)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });', 'dynamic defaults__30587 = (((object)theme__30400.useMaterial3) switch { true => (year2023__30501 ? new _SliderDefaultsM3Year2023__slider(context) : new _SliderDefaultsM3__slider(context)), false => new _SliderDefaultsM2__slider(context), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });') }
Update-GeneratedFile 'switch.g.cs' {
    param($text)
    $defaultsName = [Text.RegularExpressions.Regex]::Match($text, 'dynamic (?<name>defaults__\d+) = default!;')
    if (-not $defaultsName.Success) {
        throw 'G6 Switch defaults declaration shape drifted.'
    }
    $name = $defaultsName.Groups['name'].Value
    $text = $text.ReplaceGeneratedLocalPattern(
        "(Color)$name.trackOutlineColor?.resolve",
        "(Color)((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>)$name.trackOutlineColor).resolve")
    $text = $text.ReplaceGeneratedLocalPattern('return ((_MaterialSwitch__switch)(object?)((_MaterialSwitch__switch)this.widget).inactiveThumbColor);', 'return ((_MaterialSwitch__switch)this.widget).inactiveThumbColor;')
    $text = $text.ReplaceGeneratedLocalPattern('return ((_MaterialSwitch__switch)(object?)((_MaterialSwitch__switch)this.widget).activeThumbColor);', 'return ((_MaterialSwitch__switch)this.widget).activeThumbColor;')
    $text = $text.ReplaceGeneratedLocalPattern('return ((_MaterialSwitch__switch)(object?)((_MaterialSwitch__switch)this.widget).activeTrackColor);', 'return ((_MaterialSwitch__switch)this.widget).activeTrackColor;')
    $text = $text.ReplaceGeneratedLocalPattern('return ((_MaterialSwitch__switch)(object?)((_MaterialSwitch__switch)this.widget).inactiveTrackColor);', 'return ((_MaterialSwitch__switch)this.widget).inactiveTrackColor;')
    $text = $text.ReplaceGeneratedLocalPattern('return ((((WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(((_MaterialSwitch__switch)this.widget).mouseCursor, states) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)switchTheme__32572.mouseCursor?.resolve(states))) ?? defaults__32785.mouseCursor!.resolve(states)!));', "return WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(((_MaterialSwitch__switch)this.widget).mouseCursor, states)`n    ?? switchTheme__32572.mouseCursor?.resolve(states)`n    ?? global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable.resolve(states);")
    return $text
}

# The pinned Cupertino sources provide a contextual WidgetStateProperty<T>
# return type that the current independent-batch lowerer cannot yet retain.
# Keep these adaptations exact and drift-checked so they remain owned debt,
# rather than silently accepting invalid widget-return casts.
Get-ChildItem -LiteralPath $cupertinoRoot -File -Filter '*.g.cs' | ForEach-Object {
    Update-CupertinoGeneratedFile -Name $_.Name `
        -Owner 'generated framework header compatibility' `
        -RemovalCondition 'promoted Cupertino framework headers adopt the lowerer Stopwatch alias' `
        -Transform {
        param($text)
        # Preserve the promoted G6-3 header; application output keeps the
        # safer lowerer-level alias because it can instantiate Material Switch.
        return $text.Replace('using Stopwatch = System.Diagnostics.Stopwatch;', 'using System.Diagnostics;')
    }
}

Update-CupertinoGeneratedFile 'checkbox.g.cs' {
    param($text)
    $text = Replace-CupertinoProperty $text '_defaultFillColor' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultFillColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return CupertinoColors.white.withOpacity(0.5);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoCheckbox)this.widget).activeColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultFillColor, this.context);
}
return CupertinoColors.white;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    $text = Replace-CupertinoProperty $text '_defaultCheckColor' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultCheckColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) => {
if ((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled) && states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected)))
{
    return ((CupertinoCheckbox)this.widget).checkColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledCheckColor, this.context);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoCheckbox)this.widget).checkColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultCheckColor, this.context);
}
return CupertinoColors.white;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    $text = Replace-CupertinoProperty $text '_defaultSide' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide> _defaultSide
    {
        get
        {
            return WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Painting.BorderSide>((states) => {
if ((((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected) || states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))) && !states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled)))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 0.0, color: CupertinoColors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledBorderColor, this.context));
}
return new global::Doroti.Generated.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultBorderColor, this.context));
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    return $text
}

Update-CupertinoGeneratedFile 'radio.g.cs' {
    param($text)
    $text = Replace-CupertinoProperty $text '_defaultOuterColor' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultOuterColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return CupertinoDynamicColor.resolve(RadioLibrary._kDisabledOuterColor, this.context);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return widget.activeColor ?? CupertinoDynamicColor.resolve(RadioLibrary._kDefaultOuterColor, this.context);
}
return widget.inactiveColor ?? CupertinoColors.white;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    $text = Replace-CupertinoProperty $text '_defaultInnerColor' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultInnerColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) => {
if ((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled) && states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected)))
{
    return widget.fillColor ?? CupertinoDynamicColor.resolve(RadioLibrary._kDisabledInnerColor, this.context);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return widget.fillColor ?? CupertinoDynamicColor.resolve(RadioLibrary._kDefaultInnerColor, this.context);
}
return CupertinoColors.white;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    $text = Replace-CupertinoProperty $text '_defaultBorderColor' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultBorderColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) => {
if ((((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected) || states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))) && !states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled)))
{
    return CupertinoColors.transparent;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledBorderColor, this.context);
}
return CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultBorderColor, this.context);
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    return $text
}

Update-CupertinoGeneratedFile 'switch.g.cs' {
    param($text)
    $text = Replace-CupertinoProperty $text '_widgetThumbColor' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetThumbColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoSwitch)this.widget).thumbColor;
}
return ((CupertinoSwitch)this.widget).inactiveThumbColor;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    $text = Replace-CupertinoProperty $text '_widgetTrackColor' @'
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetTrackColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoSwitch)this.widget).activeTrackColor;
}
return ((CupertinoSwitch)this.widget).inactiveTrackColor;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
'@
    return $text
}

Update-CupertinoGeneratedFile 'context_menu_action.g.cs' {
    param($text)
    $before = 'onTapCancel: this.onTapCancel'
    $matches = [Text.RegularExpressions.Regex]::Matches($text, [Text.RegularExpressions.Regex]::Escape($before))
    if ($matches.Count -ne 1) { throw "G7-2 Cupertino context menu callback shape drifted: expected 1 match, got $($matches.Count)." }
    return $text.Replace($before, 'onTapCancel: () => this.onTapCancel()')
}

Update-CupertinoGeneratedFile 'text_form_field_row.g.cs' {
    param($text)
    $before = 'public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextFormFieldRowState__text_form_field_row());'
    $after = 'public override global::Doroti.Generated.Framework.Widgets.FormFieldState<string> createState() => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FormFieldState<string>>(new _CupertinoTextFormFieldRowState__text_form_field_row());'
    $matches = [Text.RegularExpressions.Regex]::Matches($text, [Text.RegularExpressions.Regex]::Escape($before))
    if ($matches.Count -ne 1) { throw "G7-2 Cupertino FormField State return shape drifted: expected 1 match, got $($matches.Count)." }
    return $text.Replace($before, $after)
}

$report = [ordered]@{
    schemaVersion = 'doroti.g5-4-reviewed-adaptations/v1'
    milestone = 'G5-4'
    changedFiles = $changes.Count
    declarationOrFileRemovals = 0
    changes = @($changes)
}
$json = ($report | ConvertTo-Json -Depth 8) -replace "`r`n", "`n"
[IO.File]::WriteAllText((Join-Path $OutputRoot 'g5-4-reviewed-adaptations.json'), $json + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G5-4 reviewed adaptations: PASS ($($changes.Count) files, 0 declaration/file removals)"
