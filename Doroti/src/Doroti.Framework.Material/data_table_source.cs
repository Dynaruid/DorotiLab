// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/data_table_source.dart
namespace Doroti.Framework.Material;

public abstract class DataTableSource : global::Doroti.Framework.Foundation.ChangeNotifier
{
    public abstract DataRow? getRow(long index);
    public abstract long rowCount { get; }
    public abstract bool isRowCountApproximate { get; }
    public abstract long selectedRowCount { get; }
}
