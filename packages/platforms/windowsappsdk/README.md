# Windows App SDK provider

Host/Target identities remain unchanged. This provider owns the HwndExactCpp native implementation, ABI headers and native build tool. The application framework session and owner outlive the primary native loop.

Version: 0.4.0-beta. Core: [0.4.0-alpha.1, 0.5.0). Native ABI remains v1; managed application/view identities do not change the native-local view identifier.

Repository builds explicitly consume the native source operation. Packed consumers use prebuilt RID assets and never execute this source target. Actual support and test limits are recorded in the design-platform migration evidence.

Managed/native ABI is 4. Process preparation is reference counted across all native windows; generated startup runs Configure after the preparation lease is acquired. Packaged DLL consumers use the native module directory to identify self-contained Windows App Runtime assets.
