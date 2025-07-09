// CodeStyle.cs
// Code Style Guidelines for Unity Project
// Place this script as a reference only. Do not include in builds.

#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Code style guide for Unity C# projects.
/// This file is intended for team reference only.
/// </summary>
class CodeStyle
{
    /*
   Naming Conventions:
   --------------------
   - PascalCase: Classes, Structs, Enums, Methods, Public Fields/Properties
   - camelCase: Local variables, parameters, Serialized private fields (use [SerializeField])
   - _camelCase: Private and internal fields
   - UPPER_CASE: Constants and static readonly fields

   Example:
   public class PlayerController : MonoBehaviour
   {
       [SerializeField] private float moveSpeed = 5f;
       private Rigidbody _rigidbody;
       private const float GRAVITY = -9.81f;

       private void Awake()
       {
           _rigidbody = GetComponent<Rigidbody>();
       }

       private void Move(Vector3 direction)
       {
           _rigidbody.velocity = direction * moveSpeed;
       }
   }

   Spacing:
   --------
   - One blank line between methods.
   - No blank lines inside methods unless separating logical blocks.
   - Use 4 spaces for indentation (no tabs).

   Braces:
   -------
   - Always use braces, even for single-line blocks.

   Good:
   if (isRunning)
   {
       Run();
   }

   Bad:
   if (isRunning)
       Run();

   File Structure:
   ---------------
   1. Using statements
   2. Namespace (if used)
   3. Class declaration
   4. Fields (serialized, public, private)
   5. Properties
   6. Unity lifecycle methods (Awake, Start, Update, etc.)
   7. Public methods
   8. Private methods
   9. Nested classes/structs/enums

   Unity Specific:
   ---------------
   - Avoid using Update() unless necessary.
   - Prefer UniTasks, coroutines, events, or reactive patterns.
   - Use [Header], [Tooltip], [SerializeField], and [ContextMenu] to enhance inspector usability.
   - Use ScriptableObjects for shared/config data.
   - Avoid using `GameObject.Find` or `FindObjectOfType` in production code.

   Comments:
   ---------
   - Use XML-style documentation for public members.
   - Use // for inline comments, no block comments for logic.
   */

    // This class is intentionally left empty as a style guide.


    /// <summary>
    /// Press Ctrl + Alt + Q to see XML summary
    /// </summary>
    private bool _summaryHotKey;

    /// <summary>
    /// Возможные состояния объекта:
    /// </summary>
    /// <list type="bullet">
    /// <item><description>Idle — ничего не делает</description></item>
    /// <item><description>Moving — движется</description></item>
    /// <item><description>Dead — уничтожен</description></item>
    /// </list>
    private bool _summaryExample1;

    /// <summary>
    /// Устанавливает скорость перемещения.
    /// </summary>
    /// <param name="speed">Скорость движения в м/с</param>
    /// <remarks>
    /// Вызывает <see cref="UpdatePosition"/>.
    /// </remarks>
    /// <code>
    /// SetSpeed(5.0f);
    /// </code>
    private bool _summaryExample2;
}
#endif