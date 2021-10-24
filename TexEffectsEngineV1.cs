using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// I didn't had the skills to figure out how to do without setting meshes, and they block each other so only had the possibility to mix stuff terribly.
public class TextEffectEngine : MonoBehaviour
{
    TMP_Text textComponent;
    Mesh mesh;
    string activeVoidName;

    [Header("Effects")]
    public bool usingLetterWobble;
    public bool usingWordWobble;
    public bool usingLetterDeform;
    public bool usingFullWave;
    public bool usingColorModification;

    // Colors utilities 
    Vector3[] vertices;
    List<int> wordIndexes;
    List<int> wordLengths;
    public Gradient rainbow;
    [Header("Wave effect values")]
    public float waveSpeed = 2f;
    public float waveHeight = 10f;

    [Header("Wobble effect values")]
    public float hMovementSin = 3.3f;
    public float vMovementCos = 2.5f;

    [Header("Deform effect values")]
    public float hDeformSin = 3.3f;
    public float vDeformCos = 2.5f;

    [Header("Universal effect values")]
    public float hMultiply = 3.3f;
    public float vMultiply = 2.5f;
    public float deformMultiply = 1f;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();

        if(usingWordWobble)
        {
            wordIndexes = new List<int>{0};
            wordLengths = new List<int>();

            string s = textComponent.text;
            for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
            {
                    wordLengths.Add(index - wordIndexes[wordIndexes.Count - 1]);
                    wordIndexes.Add(index + 1);
            }
            wordLengths.Add(s.Length - wordIndexes[wordIndexes.Count - 1]);
        }

        voidCalledDefiner();
    }

    void voidCalledDefiner()
    {
        if(usingLetterWobble && usingLetterDeform && usingColorModification && usingFullWave)
        {
            activeVoidName = "letterWobbleWaveDeformColorModif";
        }
        else if (usingWordWobble && usingLetterDeform && usingColorModification && usingFullWave)
        {
            activeVoidName = "wordWobbleWaveDeformColorModif";
        }
        else if(usingLetterWobble && usingLetterDeform && usingColorModification)
        {
            activeVoidName = "letterWobbleDeformColor";
        }
        else if(usingWordWobble && usingLetterDeform && usingColorModification)
        {
            activeVoidName = "wordWobbleDeformColor";
        }
        else if(usingFullWave && usingLetterDeform && usingColorModification)
        {
            activeVoidName = "waveDeformColor";
        }
        else if(usingFullWave && usingColorModification)
        {
            activeVoidName = "waveColor";
        }
        else if(usingLetterWobble && usingColorModification)
        {
            activeVoidName = "letterWobbleColor";
        }
        else if(usingWordWobble && usingColorModification)
        {
            activeVoidName = "wordWobbleColor";
        }
        else if(usingFullWave && usingLetterDeform)
        {
            activeVoidName = "waveDeform";
        }
        else if(usingLetterWobble && usingLetterDeform)
        {
            activeVoidName = "letterWobbleDeform";
        }
        else if(usingWordWobble && usingLetterDeform)
        {
            activeVoidName = "wordWobbleDeform";
        }
        else if(usingFullWave)
        {
            activeVoidName = "FullWave";
        }
        else if(usingWordWobble & !usingLetterWobble)
        {
            activeVoidName = "WordWobble";
        }

        else if(usingColorModification)
        {
            activeVoidName = "ColorModification";
        }
        else if(usingLetterDeform)
        {
            activeVoidName = "LetterDeform";
        }
        else if(usingLetterWobble & !usingWordWobble)
        {
            activeVoidName = "LetterWobble";
        }
        else
        {
            Debug.Log("Incompatible effects have been chosen in Text Effect Engine.");
        }
        print(activeVoidName);
    }

    void Update()
    {
        if(activeVoidName != null)
        {
            Invoke(activeVoidName, 0f);    
        }
    }

    void waveDeformColor()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        //color
        Color[] colors = mesh.colors;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter wobble
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;

            //color
            colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
            colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
            colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
            colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));

            if(!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            //wave effect
            for(int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*waveSpeed + orig.x*0.01f) * waveHeight, 0);
            }
        }

        for(int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
        } 

        // Deform
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);

            vertices[i] = vertices[i] + offset * deformMultiply;
        }

        //color
        mesh.colors = colors;
        textComponent.mesh.colors = mesh.colors;
        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void waveDeform()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter wobble
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            
            if(!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            //wave effect
            for(int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*waveSpeed + orig.x*0.01f) * waveHeight, 0);
            }
        }

        for(int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            //textComponent.UpdateGeometry(meshInfo.mesh, i);
        } 

        // Deform
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);

            vertices[i] = vertices[i] + offset * deformMultiply;
        }

        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void letterWobbleDeform()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter wobble
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            Vector3 offset = Wobble(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        // Deform
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);
            vertices[i] = vertices[i] + offset * deformMultiply;
        }

        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void wordWobbleDeform()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        //color
        Color[] colors = mesh.colors;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //word wobble
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            Vector3 offset = Wobble(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset; 
        }
        // Deform
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);

            vertices[i] = vertices[i] + offset * deformMultiply;
        }
        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void waveColor()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        //color array
        Color[] colors = mesh.colors;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter getter
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;

            //color
            colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
            colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
            colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
            colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));

            if(!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            //wave effect
            for(int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * waveSpeed + orig.x*0.01f) * waveHeight, 0);
            }
        }

        for(int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            //textComponent.UpdateGeometry(meshInfo.mesh, i);
        } 

        //color
        mesh.colors = colors;
        textComponent.mesh.colors = mesh.colors;

        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void letterWobbleColor()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        //color
        Color[] colors = mesh.colors;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter wobble
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            Vector3 offset = Wobble(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;

            //color
            colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
            colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
            colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
            colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));
        }
        //color
        mesh.colors = colors;
        textComponent.mesh.colors = mesh.colors;
        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void wordWobbleColor()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        vertices = mesh.vertices;
        //colors
        Color[] colors = mesh.colors;

        for (int w = 0; w < wordIndexes.Count; w++)
        {
            int wordIndex = wordIndexes[w];
            Vector3 offset = Wobble(Time.time + w);

            for (int i = 0; i < wordLengths[w]; i++)
            {
                TMP_CharacterInfo c = textComponent.textInfo.characterInfo[wordIndex+i];
                int index = c.vertexIndex;

                // Movement
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;  

                //color
                colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
                colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
                colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
                colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));                
            }
        }
        //color
        mesh.colors = colors;
        textComponent.mesh.colors = mesh.colors;
        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void letterWobbleWaveDeformColorModif()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        //color
        Color[] colors = mesh.colors;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter wobble
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            Vector3 offset = Wobble(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;

            //color
            colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
            colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
            colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
            colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));

            if(!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            //wave effect
            for(int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*waveSpeed + orig.x*0.01f) * waveHeight, 0);
            }
        }

        for(int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            //textComponent.UpdateGeometry(meshInfo.mesh, i);
        } 

        // Deform
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);

            vertices[i] = vertices[i] + offset * deformMultiply;
        }

        //color
        mesh.colors = colors;
        textComponent.mesh.colors = mesh.colors;
        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void wordWobbleWaveDeformColorModif()
    {

    }


    void letterWobbleDeformColor()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        //color
        Color[] colors = mesh.colors;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter wobble
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            Vector3 offset = Deform(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;

            //color
            colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
            colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
            colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
            colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));

            if(!charInfo.isVisible)
            {
                continue;
            }
        }
        // Deform
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);

            vertices[i] = vertices[i] + offset * deformMultiply;
        }

        //color
        mesh.colors = colors;
        textComponent.mesh.colors = mesh.colors;
        //wobble update
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void wordWobbleDeformColor()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        vertices = mesh.vertices;
        
        Color[] colors = mesh.colors;

        for (int w = 0; w < wordIndexes.Count; w++)
        {
            int wordIndex = wordIndexes[w];
            Vector3 offset = Wobble(Time.time + w);

            for (int i = 0; i < wordLengths[w]; i++)
            {
                TMP_CharacterInfo c = textComponent.textInfo.characterInfo[wordIndex+i];
                int index = c.vertexIndex;

                // Movement
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;  

                // color
                colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
                colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
                colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
                colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));                
            }
        }

        // Deform
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);

            vertices[i] = vertices[i] + offset * deformMultiply;
        }
        mesh.colors = colors;
        mesh.vertices = vertices;
        textComponent.mesh.colors = mesh.colors;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void FullWave()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if(!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for(int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*waveSpeed + orig.x*0.01f) * waveHeight, 0);
            }
        }

        for(int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }


    void ColorModification()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        var textInfo = textComponent.textInfo;
        //verts for calc
        vertices = mesh.vertices;

        //color
        Color[] colors = mesh.colors;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            //letter getter
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];
            int index = c.vertexIndex;

            //color
            colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
            colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
            colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
            colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));
        }

        //color
        mesh.colors = colors;
        textComponent.mesh.colors = mesh.colors;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void LetterWobble()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textComponent.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textComponent.textInfo.characterInfo[i];

            int index = c.vertexIndex;

            Vector3 offset = Wobble(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void LetterDeform()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = Deform(Time.time + i);

            vertices[i] = vertices[i] + offset * deformMultiply;
        }

        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }


    void WordWobble()
    {
        textComponent.ForceMeshUpdate();
        mesh = textComponent.mesh;
        vertices = mesh.vertices;

        for (int w = 0; w < wordIndexes.Count; w++)
        {
            int wordIndex = wordIndexes[w];
            Vector3 offset = Wobble(Time.time + w);

            for (int i = 0; i < wordLengths[w]; i++)
            {
                TMP_CharacterInfo c = textComponent.textInfo.characterInfo[wordIndex+i];
                int index = c.vertexIndex;

                // Movement
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;                
            }
        }
        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }



    Vector2 Wobble(float time) 
    {
        return new Vector2((Mathf.Sin(time*hMovementSin))*hMultiply, Mathf.Sin((time*vMovementCos)*vMultiply));
    }

    Vector2 Deform(float time) 
    {
        return new Vector2(Mathf.Sin(time*hDeformSin), Mathf.Cos(time*vDeformCos));
    }
}
