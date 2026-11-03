#   Debug sample

Import through Package Manager, open Sample Scene and enter Play Mode. The profile installs the Debug service and its Console output. Buttons exercise severity, Trace caller labels, exceptions and Enabled filtering.

Add UI Output retains Console and registers one optional UnityUIDebugger. Remove UI Output releases only that target. The sample applies maxLines to the target, uses the current built-in runtime font and destroys only generated UI; borrowed profile Canvas/Text survive. Disable/re-enable demonstrates subscription cleanup and reattachment. The sample screen remains consumer code, while UnityUIDebugger is the package's supported output implementation.
