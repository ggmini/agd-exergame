using UnityEngine;
using UnityEditor;

public class GridArrange
{
    [MenuItem("Tools/Arrange Selected Into Grid")]
    static void Arrange()
    {
        var objects = Selection.transforms;

        if (objects.Length == 0)
        {
            Debug.Log("No objects selected.");
            return;
        }

        int columns = 30;
        float spacing = 1f; // Distance between objects

        for (int i = 0; i < objects.Length; i++)
        {
            int x = i % columns;
            int z = i / columns;

            Undo.RecordObject(objects[i], "Arrange Grid");

            objects[i].position = new Vector3(
                x * spacing,
                objects[i].position.y,
                z * spacing
            );
        }

        Debug.Log($"Arranged {objects.Length} objects.");
    }
}