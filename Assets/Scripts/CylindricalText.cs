using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{

    public class CylindricalText : MonoBehaviour
    {

        private TMP_Text m_TextComponent;

        public float radius;


        void Awake()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
        }


        void Start()
        {
            StartCoroutine(WarpText());
        }


        private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
        {
            AnimationCurve newCurve = new AnimationCurve();

            newCurve.keys = curve.keys;

            return newCurve;
        }


        /// <summary>
        ///  Method to curve text along a Unity animation curve.
        /// </summary>
        /// <param name="textComponent"></param>
        /// <returns></returns>
        IEnumerator WarpText()
        {
            

            Vector3[] vertices;
            Matrix4x4 matrix;

            m_TextComponent.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.




            while (true)
            {
                if (!m_TextComponent.havePropertiesChanged)
                {
                    yield return null;
                    continue;
                }


                m_TextComponent.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.

                TMP_TextInfo textInfo = m_TextComponent.textInfo;
                int characterCount = textInfo.characterCount;


                if (characterCount == 0) continue;


                float boundsMinX = m_TextComponent.bounds.min.x;  //textInfo.meshInfo[0].mesh.bounds.min.x;
                float boundsMaxX = m_TextComponent.bounds.max.x;  //textInfo.meshInfo[0].mesh.bounds.max.x;

                float totalWidthOfText = (boundsMaxX - boundsMinX);



                for (int i = 0; i < characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible)
                        continue;

                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Get the index of the mesh used by this character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                    vertices = textInfo.meshInfo[materialIndex].vertices;

                    // Compute the baseline mid point for each character
                    Vector3 offsetToMidBaseline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);



                    // Apply offset to adjust our pivot point.
                    vertices[vertexIndex + 0] += -offsetToMidBaseline;
                    vertices[vertexIndex + 1] += -offsetToMidBaseline;
                    vertices[vertexIndex + 2] += -offsetToMidBaseline;
                    vertices[vertexIndex + 3] += -offsetToMidBaseline;



                    //need to find out the arc length of each character's position on the circle

                    float charPos = (offsetToMidBaseline.x - boundsMinX);

                    float midpoint = (boundsMaxX - boundsMinX) * 0.5f;

                    float offsetAngle = (((3.14f * radius) - totalWidthOfText) * 0.5f) / radius;


                    float angleToChar = charPos / radius; //this is in radians

                    angleToChar += offsetAngle;

                    float xPosOfChar = radius * Mathf.Cos(angleToChar);
                    float yPosOfChar = radius * Mathf.Sin(angleToChar);


                    float angleToCharDeg = angleToChar * 180 / 6.283f;

                    matrix = Matrix4x4.TRS( new Vector3( midpoint - charPos ,0,0) + new Vector3(-xPosOfChar,0,-yPosOfChar) , Quaternion.Euler(0,90 - (2*angleToCharDeg),0), Vector3.one);


                    vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                    vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                    vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                    vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

                    vertices[vertexIndex + 0] += offsetToMidBaseline;
                    vertices[vertexIndex + 1] += offsetToMidBaseline;
                    vertices[vertexIndex + 2] += offsetToMidBaseline;
                    vertices[vertexIndex + 3] += offsetToMidBaseline;
                }


                // Upload the mesh with the revised information
                m_TextComponent.UpdateVertexData();

                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}
