using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Objektauswahl : EditorWindow
{
    private string nameFilter = ""; // Default value
    private ComponentFilter selectedComponentFilter = ComponentFilter.IgnoreComponent;
    private bool includeInactive = false;
    private string statusText = ""; // Status text to display
    private GUIStyle statusStyle;

    private GameObject parentGameObject; // New field for the parent GameObject

    // Enum for component filter options
    public enum ComponentFilter
    {
        IgnoreComponent,
        None,
        Image,
        Light,
        Collider,
        LODGroup,
        BoxCollider,
        MeshRenderer,
        MeshCollider,
        ReflectionProbe,
        LightProbeGroup,
    }

    [MenuItem("Tools/select objects with filter")]
    public static void ShowWindow()
    {
        GetWindow<Objektauswahl>("Object Selection");
    }

    private void OnEnable()
    {
        statusStyle = new GUIStyle();
        statusStyle.normal.textColor = Color.red;
        statusStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnGUI()
    {
        GUI.backgroundColor = new Color(0.2f, 0.3f, 0.4f, 1f);
        GUILayout.BeginVertical("box");

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.normal.textColor = Color.white;
        GUILayout.Label("Select objects using the filter", headerStyle);

        GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        EditorGUI.BeginChangeCheck();
        nameFilter = EditorGUILayout.TextField("Name", nameFilter);
        if (EditorGUI.EndChangeCheck())
            statusText = "";

        GUI.backgroundColor = new Color(0.2f, 0.4f, 0.2f, 1f);
        EditorGUI.BeginChangeCheck();
        selectedComponentFilter = (ComponentFilter)EditorGUILayout.EnumPopup("Component", selectedComponentFilter);
        if (EditorGUI.EndChangeCheck())
            statusText = "";

        // toggle for including inactive GameObjects
        GUI.backgroundColor = new Color(1f, 0.8f, 0.8f, 1f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("select inactive");
        GUILayout.FlexibleSpace();
        EditorGUI.BeginChangeCheck();
        includeInactive = EditorGUILayout.Toggle(includeInactive);
        if (EditorGUI.EndChangeCheck())
            statusText = "";
        GUILayout.EndHorizontal();

        // Drag and drop field for parent GameObject
        GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        EditorGUILayout.LabelField("drop optional parent-object here:");
        parentGameObject = (GameObject)EditorGUILayout.ObjectField(parentGameObject, typeof(GameObject), true);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(statusText, statusStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(0.2f, 0.5f, 0.8f, 1f);
        if (GUILayout.Button("Select Objects"))
        {
            statusText = "selecting...";
            Repaint();

            EditorApplication.delayCall += () =>
            {
                SelectObjects(nameFilter, selectedComponentFilter, parentGameObject, includeInactive);
                statusText = "Done!";
            };
        }

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Selects objects based on the specified filters.
    /// </summary>
    /// <param name="nameFilter">The name filter to apply.</param>
    /// <param name="componentFilter">The component filter to apply.</param>
    /// <param name="parentGameObject">The optional parent GameObject to filter by.</param>
    /// <param name="includeInactive">Whether to include inactive GameObjects in the selection.</param>
    private void SelectObjects(string nameFilter, ComponentFilter componentFilter, GameObject parentGameObject, bool includeInactive)
    {
        GameObject[] allGameObjects = null;

        if (includeInactive)
        {
            // Find all GameObjects, including inactive ones
            allGameObjects = CustomUtility.FindObjectsIncludingInactive();
        }
        else
        {
            // Find all active GameObjects
            allGameObjects = FindObjectsOfType<GameObject>();
        }

        var objectsToSelect = new List<GameObject>();

        foreach (GameObject go in allGameObjects)
        {
            bool shouldSelect = true;

            if (parentGameObject != null && !IsChildOfParent(go, parentGameObject))
            {
                // Check if the GameObject is a child of the specified parent
                shouldSelect = false;
            }

            if (!string.IsNullOrEmpty(nameFilter))
            {
                // Check if the GameObject's name contains the specified filter
                if (!go.name.Contains(nameFilter))
                    shouldSelect = false;
            }

            switch (componentFilter)
            {
                case ComponentFilter.IgnoreComponent:
                    // No component filter
                    break;
                case ComponentFilter.None:
                    // Check if the GameObject has no components (except Transform)
                    if (go.GetComponents<Component>().Length > 1)
                        shouldSelect = false;
                    break;
                case ComponentFilter.Image:
                    // Check if the GameObject has a MeshRenderer component
                    if (go.GetComponent<Image>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.MeshRenderer:
                    // Check if the GameObject has a MeshRenderer component
                    if (go.GetComponent<MeshRenderer>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.Collider:
                    // Check if the GameObject has a Collider component
                    if (go.GetComponent<Collider>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.BoxCollider:
                    // Check if the GameObject has a BoxCollider component
                    if (go.GetComponent<BoxCollider>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.MeshCollider:
                    // Check if the GameObject has a MeshCollider component
                    if (go.GetComponent<MeshCollider>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.LODGroup:
                    // Check if the GameObject has a LODGroup component
                    if (go.GetComponent<LODGroup>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.Light:
                    // Check if the GameObject has a Light component
                    if (go.GetComponent<Light>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.ReflectionProbe:
                    // Check if the GameObject has a ReflectionProbe component
                    if (go.GetComponent<ReflectionProbe>() == null)
                        shouldSelect = false;
                    break;
                case ComponentFilter.LightProbeGroup:
                    // Check if the GameObject has a LightProbeGroup component
                    if (go.GetComponent<LightProbeGroup>() == null)
                        shouldSelect = false;
                    break;
            }

            if (shouldSelect)
            {
                // Add the GameObject to the selection list
                objectsToSelect.Add(go);
            }
        }

        // Set the selected objects in the Unity Editor
        Selection.objects = objectsToSelect.ToArray();
    }

    private bool IsChildOfParent(GameObject child, GameObject parent)
    {
        Transform t = child.transform;
        while (t != null)
        {
            if (t.gameObject == parent)
                return true;
            t = t.parent;
        }
        return false;
    }
}