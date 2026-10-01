''' <summary>The three user types (docs/ACCESS_CONTROL.md). Operator replaces the spec's Employee.</summary>
Public Enum UserType
    Associate = 1
    Company = 2
    ' Operator is a VB keyword, so the name needs brackets.
    [Operator] = 3
End Enum
