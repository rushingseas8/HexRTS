using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    [SerializeField]
    public GameObject errorTextGameObject;

    [SerializeField]
    public TextMesh errorTextMesh;

    private List<GameObject> inWorldErrors = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> toRemove = null;
        foreach (GameObject inWorldError in inWorldErrors)
        {
            inWorldError.transform.position += 1.0f * Time.deltaTime * Vector3.up;
            if (inWorldError.transform.position.y >= 2.0f)
            {
                // Error is above a certain height. Add it to the list of pending removals.
                if (toRemove == null)
                {
                    // Optimization: lazy initialize
                    toRemove = new List<GameObject>();
                }
                toRemove.Add(inWorldError);
            }
        }

        // Remove all the elements we're about to remove.
        if (toRemove != null)
        {
            foreach (GameObject toRemoveGO in toRemove)
            {
                inWorldErrors.Remove(toRemoveGO);
                GameObject.Destroy(toRemoveGO);
            }
            toRemove.Clear();
        }
    }

    /// <summary>
    /// Creates an error message at the specified position with the specified text.
    /// </summary>
    /// <param name="worldPos">The world position</param>
    /// <param name="message">The message</param>
    public void CreateError(Vector3 worldPos, string message)
    {
        GameObject newError = GameObject.Instantiate(errorTextGameObject);
        newError.transform.position = worldPos;
        newError.GetComponent<TextMesh>().text = message;

        inWorldErrors.Add(newError);
    }
}
