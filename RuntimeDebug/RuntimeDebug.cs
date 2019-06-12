using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;


public class RuntimeDebug : MonoBehaviour
{
    public static Color color = new Color(1.0f, 0.0f, 0.0f);

    public static RuntimeDebug Instance { get; private set; }



    private static List<LogData> logDataList = new List<LogData>();
    //private List<Line>[] lineList = new List<Line>[2];
    private static List<Line> updatedLineList = new List<Line>();
    private static List<Line> renderedLineList = new List<Line>();


    static bool frameUpdated = false;

    private class Line
    {
        public Line(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
        }
        public Vector3 from;
        public Vector3 to;
    }


    private class LogData
    {
        public enum Type
        {
            Note,
            Warning,
            Error
        }

        public Type type;

        public string message;
        //public string identifer;

        public override string ToString()
        {

            string beginStr = "";
            string endStr = "";

            switch (type)
            {
                default:
                case Type.Note:

                    beginStr = "<color=white>";
                    endStr = "</color>";

                    break;
                case Type.Error:
                    beginStr = "<color=red>";
                    endStr = "</color>";

                    break;

                case Type.Warning:

                    beginStr = "<color=yellow>";
                    endStr = "</color>";

                    break;
            }


            return beginStr + message + endStr;
        }
    }


    public Text text;


    // private bool frameUpdated = false;

    public static void LogError(string message)
    //public static void LogError(string identifer, string message)
    {
        LogData data = new LogData();

        //data.identifer = identifer;
        data.message = message;
        data.type = LogData.Type.Error;

        LogInternal(data);
    }

    // public static void LogWarning(string identifer, string message)
    public static void LogWarning(string message)
    {
        LogData data = new LogData();

        //data.identifer = identifer;
        data.message = message;
        data.type = LogData.Type.Warning;

        LogInternal(data);
    }

    //public static void Log(string identifer, string message)
    public static void Log(string message)
    {
        LogData data = new LogData();

        //data.identifer = identifer;
        data.message = message;
        data.type = LogData.Type.Note;

        LogInternal(data);
    }

    private static void LogInternal(LogData logDataToAdd)
    {
        logDataList.Add(logDataToAdd);

        //if (instance.logDataList.ContainsKey(logDataToAdd.identifer))
        //{
        //    instance.logDataList[logDataToAdd.identifer] = logDataToAdd;

        //}
        //else
        //{
        //    instance.logDataList.Add(logDataToAdd.identifer, logDataToAdd);
        //}
    }


    public static void DrawLine(Vector3 from, Vector3 to)
    {
        updatedLineList.Add(new Line(from, to));
    }


    public static void DrawWireCube(Vector3 center, Vector3 size)
    {
        Vector3 from = center - size / 2.0f;
        Vector3 to = from + new Vector3(size.x, 0.0f, 0.0f);

        DrawLine(from, to);

        from = to;
        to = from + new Vector3(0.0f, 0.0f, size.z);
        DrawLine(from, to);




    }
    private void Awake()
    {
        //logDataList = new Dictionary<string, LogData>();
        //Debug.Log(Instance);

        if (!text)
        {
            text = GetComponent<Text>();
        }

        Instance = this;
    }


    void Start()
    {
        if (!text)
        {
            enabled = false;

            return;
        }




    }





    void LateUpdate()
    {
        string textStr = "";



        //foreach (var logDataKeyValue in logDataList)
        //{
        //    LogData logData = logDataKeyValue.Value;


        //    textStr += logData.ToString() + "\n";
        //}

        foreach (var logData in logDataList)
        {


            textStr += logData.ToString() + "\n";
        }

        text.text = textStr;

        logDataList.Clear();


        List<Line> temp = updatedLineList;
        updatedLineList = renderedLineList;
        renderedLineList = temp;

        updatedLineList.Clear();
        //frameCount = (++frameCount) % FrameCountCycle;
    }



    static Material lineMaterial;



    // Use this for initialization

    static void CreateLineMaterial()
    {

        if (!lineMaterial)
        {

            // Unity has a built-in shader that is useful for drawing

            // simple colored things.

            Shader shader = Shader.Find("Hidden/Internal-Colored");

            lineMaterial = new Material(shader);

            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            // Turn on alpha blending

            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);

            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            // Turn backface culling off

            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

            // Turn off depth writes

            lineMaterial.SetInt("_ZWrite", 0);

        }

    }



    public void OnRenderObject()
    {

        CreateLineMaterial();

        // Apply the line material
        lineMaterial.SetPass(0);



        GL.PushMatrix();

        // Set transformation matrix for drawing to
        // match our transform
        //GL.MultMatrix(transform.localToWorldMatrix);

        //GL.LoadIdentity();




        GL.Begin(GL.LINES);

        GL.Color(color);
        foreach (var line in renderedLineList)
        {

            GL.Vertex(line.from);
            GL.Vertex(line.to);

        }

        GL.End();

        //int lineCount = 100;
        //float radius = 3.0f;

        //GL.Begin(GL.LINES);
        //for (int i = 0; i < lineCount; ++i)
        //{
        //    float a = i / (float)lineCount;
        //    float angle = a * Mathf.PI * 2;
        //    // Vertex colors change from red to green
        //    GL.Color(new Color(a, 1 - a, 0, 0.8F));
        //    // One vertex at transform position
        //    GL.Vertex3(0, 0, 0);
        //    // Another vertex at edge of circle
        //    GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        //}
        //GL.End();




        GL.PopMatrix();

        //Debug.Log(renderedLineList.Count);
    }

    //    protected static readonly Vector3[] VertexOffset = new Vector3[] {
    //        new Vector3(0.0f,0.0f,0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1)
    //    {0, 0, 0}, {1, 0, 0}, {1, 1, 0}, {0, 1, 0},
    //    {0, 0, 1}, {1, 0, 1}, {1, 1, 1}, {0, 1, 1}
    //};


}


