using UnityEngine;

public class LineGen : MonoBehaviour
{
    [Header("Materials")]
    public Material cubeMaterial;       
    public Material platformMaterial;  

    [Header("Cube")]
    public float cubeSize = 1f;
    public Vector2 cubePos = new Vector2(0, 0);
    public float zPos = 0;

    [Header("Jump")]
    private float velocityY = 0f;
    public float gravity = -10f;
    public float jumpStrength = 10f;
    private bool grounded = false;

    [Header("Platform")]
    public Vector2 platformPos = new Vector2(0, -3);
    public float platformWidth = 10f;
    public float platformHeight = 1f;
    public float platformZPos = 0;

    private void Update()
    {
        ApplyPhysics();
    }

    private void OnPostRender()
    {
        DrawCube();
    }

    //Jump&Fall
    void ApplyPhysics()
    {
        velocityY += gravity * Time.deltaTime;
        float nextY = cubePos.y + velocityY * Time.deltaTime;

        float platformTop = platformPos.y + platformHeight + cubeSize;

        if (velocityY < 0 && nextY - cubeSize <= platformPos.y + platformHeight &&
            cubePos.x + cubeSize >= platformPos.x - platformWidth &&
            cubePos.x - cubeSize <= platformPos.x + platformWidth)
        {
            cubePos.y = platformPos.y + platformHeight + cubeSize;
            velocityY = 0f;
            grounded = true;
            cubeMaterial.color = Color.green;
        }
        else
        {
            cubePos.y = nextY;
            grounded = false;
            cubeMaterial.color = Color.red;
        }

        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            grounded = false;
            velocityY = jumpStrength;
        }
    }

    //DrawCube
    void DrawCube()
    {
        GL.PushMatrix();

        platformMaterial.SetPass(0);
        GL.Begin(GL.LINES);

        var pFront = GetRect(platformPos, platformWidth, platformHeight);
        var pBack = GetRect(platformPos, platformWidth, platformHeight);

        float pFrontZ = PerspectiveCamera.Instance.GetPerspective(platformZPos + 1);
        float pBackZ = PerspectiveCamera.Instance.GetPerspective(platformZPos - 1);

        Vector2[] pF = RenderSquare(pFront, pFrontZ);
        Vector2[] pB = RenderSquare(pBack, pBackZ);

        for (int i = 0; i < pF.Length; i++)
        {
            GL.Vertex(pF[i]);
            GL.Vertex(pB[i]);
        }

        GL.End();

        cubeMaterial.SetPass(0);
        GL.Begin(GL.LINES);

        var cFront = GetSquare(cubePos, cubeSize);
        var cBack = GetSquare(cubePos, cubeSize);

        float cFrontZ = PerspectiveCamera.Instance.GetPerspective(zPos + cubeSize);
        float cBackZ = PerspectiveCamera.Instance.GetPerspective(zPos - cubeSize);

        Vector2[] cF = RenderSquare(cFront, cFrontZ);
        Vector2[] cB = RenderSquare(cBack, cBackZ);

        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(cF[i]);
            GL.Vertex(cB[i]);
        }

        GL.End();

        GL.PopMatrix();
    }

    Vector2[] GetSquare(Vector2 pos, float size)
    {
        return new Vector2[]
        {
            pos + new Vector2(size, size),
            pos + new Vector2(-size, size),
            pos + new Vector2(-size, -size),
            pos + new Vector2(size, -size)
        };
    }

    Vector2[] GetRect(Vector2 pos, float w, float h)
    {
        return new Vector2[]
        {
            pos + new Vector2(w, h),
            pos + new Vector2(-w, h),
            pos + new Vector2(-w, -h),
            pos + new Vector2(w, -h)
        };
    }

    Vector2[] RenderSquare(Vector2[] square, float p)
    {
        Vector2[] array = new Vector2[square.Length];
        for (int i = 0; i < square.Length; i++)
        {
            array[i] = square[i] * p;
            GL.Vertex(array[i]);
            GL.Vertex(square[(i + 1) % square.Length] * p);
        }
        return array;
    }
}

