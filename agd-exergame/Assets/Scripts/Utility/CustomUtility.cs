using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Android.Gradle;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public static class CustomUtility
{
    /// <summary>
    /// Converts a given string as "x,y,z" to a Vector3
    /// </summary>
    /// <param name="sVector"></param>
    /// <returns></returns>
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    /// <summary>
    /// Replaces all occurrences of spaces with underscores in the input string.
    /// </summary>
    /// <param name="text">The input string to be modified.</param>
    /// <returns>The modified string with spaces replaced by underscores.</returns>
    public static string ReplaceSpacesWithUnderscores(string text)
    {
        // Replace all spaces in the given string with underscores in place.
        if (text.Contains(" "))
            return text.Replace(' ', '_');

        return text;
    }

    /// <summary>
    /// Replaces all occurrences of spaces with underscores in the input string.
    /// </summary>
    /// <param name="text">The input string to be modified.</param>
    /// <returns>The modified string with spaces replaced by underscores.</returns>
    public static string ReplaceSpacesWithUnderscores(ref string text)
    {
        // Replace all spaces in the given string with underscores in place.
        if (text.Contains(" "))
            return text.Replace(' ', '_');

        return text;
    }


    /// <summary>
    /// Replaces all occurrences of underscores with spaces in the input string.
    /// </summary>
    /// <param name="text">The input string to be modified.</param>
    /// <returns>The modified string with underscores replaced by spaces.</returns>
    public static string ReplaceUnderscoresWithSpaces(string text)
    {
        // Replace all underscores in the given string with spaces in place.
        if (text.Contains("_"))
            return text.Replace('_', ' ');

        return text;
    }

    /// <summary>
    /// Replaces all occurrences of underscores with spaces in the input string.
    /// </summary>
    /// <param name="text">The input string to be modified.</param>
    /// <returns>The modified string with underscores replaced by spaces.</returns>
    public static string ReplaceUnderscoresWithSpaces(ref string text)
    {
        // Replace all underscores in the given string with spaces in place.
        if (text.Contains("_"))
            return text.Replace('_', ' ');

        return text;
    }



    /// <summary>
    /// Formats a camelCase string to a sentence-like format by adding spaces before uppercase letters.
    /// </summary>
    /// <param name="input">The camelCase string to be formatted.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatCamelCaseToSentence(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder result = new StringBuilder();

        // Ensure the first letter is uppercase
        result.Append(Char.ToUpper(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            char currentChar = input[i];

            // Add space before uppercase letters
            if (Char.IsUpper(currentChar))
            {
                result.Append(' ');
            }

            result.Append(currentChar);
        }

        return result.ToString();
    }

    /// <summary>
    /// Toggles a bool. no fancy it just value = !value;
    /// </summary>
    /// <param name="value"></param>
    public static void ToggleBool(ref bool value)
    {
        value = !value;
    }


    /// <summary>
    /// positions a given UI object to a given world object
    /// </summary>
    /// <param name="uiObject"></param>
    /// <param name="worldObject"></param>
    /// <param name="camera"></param>
    /// <param name="offset"></param>
    public static void recenterUIToWorld(GameObject uiObject, GameObject worldObject, Camera camera, Vector3 offset)
    {
        Vector3 pos = camera.WorldToScreenPoint(worldObject.transform.position + offset);

        if (uiObject.transform.position != pos)
            uiObject.transform.position = pos;
    }


    /// <summary>
    /// Finds a GameObject even if it is inactive
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public static GameObject FindObjectByNameIncludingInactive(string objectName, bool containsName = false)
    {
        // Find all root objects in the scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // Search for the object by name in the root object and its children
        foreach (GameObject rootObject in rootObjects)
        {
            Transform[] allChildren = rootObject.GetComponentsInChildren<Transform>(true);
            if (containsName)
            {
                foreach (Transform child in allChildren)
                {
                    if (child.name.Contains(objectName))
                    {
                        return child.gameObject;
                    }
                }
            }
            else
            {
                foreach (Transform child in allChildren)
                {
                    if (child.name == objectName)
                    {
                        return child.gameObject;
                    }
                }
            }
        }

        return null; // Object not found
    }

    /// <summary>
    /// Finds all GameObjects with a given name, even if they are inactive
    /// </summary>
    /// <param name="objectName">The name of the GameObjects to find</param>
    /// <returns>An array of GameObjects with the specified name</returns>
    public static GameObject[] FindObjectsByNameIncludingInactive(string objectName, bool containsName = false)
    {
        List<GameObject> foundObjects = new List<GameObject>();

        // Find all root objects in the scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();


        // Search for the object by name in the root object and its children
        foreach (GameObject rootObject in rootObjects)
        {
            Transform[] allChildren = rootObject.GetComponentsInChildren<Transform>(true);

            if (containsName)
            {
                foreach (Transform child in allChildren)
                {
                    if (child.name.Contains(objectName))
                    {
                        foundObjects.Add(child.gameObject);
                    }
                }
            }
            else
            {
                foreach (Transform child in allChildren)
                {
                    if (child.name == objectName)
                    {
                        foundObjects.Add(child.gameObject);
                    }
                }
            }
        }

        return foundObjects.ToArray();
    }


    /// <summary>
    /// Find all GameObjects of a given type, even if they are inactive
    /// </summary>
    /// <returns></returns>
    public static GameObject[] FindObjectsIncludingInactive()
    {
        List<GameObject> foundObjects = new List<GameObject>();

        // Find all root objects in the scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // Search for the objects of the specified type in the root object and its children
        foreach (GameObject rootObject in rootObjects)
        {
            Transform[] allChildren = rootObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                foundObjects.Add(child.gameObject);
            }
        }

        return foundObjects.ToArray();
    }


    /// <summary>
    /// Converts the given date to a total number of passed days
    /// </summary>
    /// <param name="date"></param>
    /// <returns> the amount of days that have passed since the w0 d1 m1 y1</returns>
    //public static int ToPassedDays(Date date)
    //{
    //    // Calculate the total number of days passed since the starting date
    //    int yearsPassed = date.year - 1;
    //    int monthsPassed = (yearsPassed * 365) + ((yearsPassed + 3) / 4) - ((yearsPassed + 99) / 100) + ((yearsPassed + 399) / 400);
    //    int daysPassed = monthsPassed;
    //
    //    // Calculate the days passed for each month
    //    int[] monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    //    for (int i = 1; i < date.month; i++)
    //    {
    //        daysPassed += monthDays[i - 1];
    //    }
    //
    //    // Add the remaining days of the current month
    //    daysPassed += date.day - 1;
    //
    //    return daysPassed;
    //}
    //
    ///// <summary>
    ///// checks if a BlockUi and a Line have the same Blocktype
    ///// </summary>
    ///// <param name="blockUI"></param>
    ///// <param name="line"></param>
    ///// <returns></returns>
    //public static bool SameType(BlockUI blockUI, Line line)
    //{
    //    return blockUI.block.type == line.lineType;
    //}
    //
    //public static bool SameType(Line line1, Line line2)
    //{
    //    return line1.lineType == line2.lineType;
    //}
    //
    ///// <summary>
    ///// Merges two dictionaries by adding values from the stacking dictionary into the base dictionary.
    ///// If a key from the stacking dictionary already exists in the base dictionary, its value is added to the existing value.
    ///// If a key does not exist in the base dictionary, it is added.
    ///// The resulting dictionary is sorted by key in ascending order before being returned.
    ///// </summary>
    ///// <param name="baseDictionary">The base dictionary to which values are added and returned.</param>
    ///// <param name="stackingDictionary">The dictionary containing values to be added to the base dictionary.</param>
    ///// <returns>A sorted dictionary containing the combined values of both input dictionaries.</returns>

    public static Dictionary<int, int> MergeDictionaries(Dictionary<int, int> baseDictionary, Dictionary<int, int> stackingDictionary)
    {
        foreach (var kvp in stackingDictionary)
        {
            if (baseDictionary.ContainsKey(kvp.Key))
            {
                // Add the value to the existing key.
                baseDictionary[kvp.Key] += kvp.Value;
            }
            else
            {
                // Add the new key and value.
                baseDictionary[kvp.Key] = kvp.Value;
            }
        }

        // Create a sorted dictionary as the final result (optional, if sorted order is required).
        var sorted = baseDictionary.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        baseDictionary.Clear();
        foreach (var kvp in sorted)
        {
            baseDictionary.Add(kvp.Key, kvp.Value);
        }

        return baseDictionary;
    }


}