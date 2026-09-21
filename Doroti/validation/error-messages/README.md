# Error-message terminology validation

Run from the repository root:

```powershell
python Doroti/validation/run-with-timeout.py python Doroti/validation/error-messages/verify.py
```

The check scans executable product, validation, and converter C# source while
excluding build output. It rejects the retired Dart-centric runtime messages and
also confirms that the converter retains the neutral replacement templates.
Technical identifiers that accurately name the Dart analyzer, `dart:ui`, or CLR
types are intentionally outside this wording check.
