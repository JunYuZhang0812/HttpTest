#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
public class OverdrawMonitor : MonoBehaviour
{

    #region 静态实例
    private static OverdrawMonitor instance;
	public static OverdrawMonitor Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<OverdrawMonitor>();
				if (instance == null)
				{
					var go = new GameObject("OverdrawMonitor");
					instance = go.AddComponent<OverdrawMonitor>();
				}
			}
			return instance;
		}
	}
    #endregion
    #region 成员变量
    private const float SampleTime              = 1f;
    private const int dataSize                  = 128 * 128; //Compute Shader中StructedBuffer大小，用于计算每个像素的写入次数
    private Camera m_camera                     = null;
	private RenderTexture m_overdrawTexture     = null;
	private ComputeShader m_computeShader       = null;
	private int[] m_inputData                   = new int[dataSize];
	private int[] m_resultData                  = new int[dataSize];
	private ComputeBuffer m_resultBuffer        = null;
	private Shader m_replacementShader          = null;
    private long m_accumulatedIntervalFragments = 0L;
	private float m_accumulatedIntervalOverdraw = 0f;
	private long m_intervalFrames               = 0L;
	private float m_intervalTime                = 0f;
	private bool m_disabled                     = true;
    #endregion
    #region 属性
    //当前帧写入的像素数量
    public long TotalShadedFragments { get; private set; }
	//当前帧OverDraw
	public float OverdrawRatio { get; private set; }
    //平均写入像素总数量
	public float IntervalAverageShadedFragments { get; private set; }
    //平均overdraw数量
	public float IntervalAverageOverdraw { get; private set; }
    //平均overdraw数量
	public float AccumulatedAverageOverdraw { get { return m_accumulatedIntervalOverdraw / m_intervalFrames; } }

    //最大overdraw数量
	public float MaxOverdraw { get; private set; }

	public RenderTexture OverdrawTexture { get { return m_overdrawTexture; } }
    #endregion
    #region Unity回调
    public void Awake()
	{
        //关闭图形卡的模拟
		UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/No Emulation");
		m_replacementShader = Shader.Find("DoubleGame/Debug/Overdraw");
	    RecreateCamera();
		RecreateTexture();
		RecreateComputeBuffer();
        for (int i = 0; i < m_inputData.Length; i++)
        {
            m_inputData[i] = 0;
        }
	}
	public void OnEnable()
	{
		m_disabled = false;
	}
	public void OnDisable()
	{
		m_disabled = true;
		OnDestroy();
	}
	public void LateUpdate()
	{
        if (m_disabled)
        {
            return;
        }

        RecreateCamera();
        RecreateTexture();
        Camera main = Camera.main;
        if(main != null)
        {
		    m_camera.CopyFrom(main);
		    m_camera.clearFlags = CameraClearFlags.SolidColor;
		    m_camera.backgroundColor = Color.black;
		    m_camera.targetTexture = m_overdrawTexture;
		    m_camera.SetReplacementShader(m_replacementShader, null);

		    transform.position = main.transform.position;
		    transform.rotation = main.transform.rotation;

		    m_intervalTime += Time.deltaTime;
		    if (m_intervalTime > SampleTime)
		    {
			    IntervalAverageShadedFragments = (float)m_accumulatedIntervalFragments / m_intervalFrames;
			    IntervalAverageOverdraw = (float)m_accumulatedIntervalOverdraw / m_intervalFrames;

			    m_intervalTime -= SampleTime;

			    m_accumulatedIntervalFragments = 0;
			    m_accumulatedIntervalOverdraw = 0;
			    m_intervalFrames = 0;
		    }
        }
	}
	public void OnDestroy()
	{
		if (m_camera != null)
		{
			m_camera.targetTexture = null;
		}
        if (m_resultBuffer != null)
        {
            m_resultBuffer.Release();
        }
	}
	public void OnPostRender()
	{
        if (m_disabled)
        {
            return;
        }
        RecreateComputeShader();
        RecreateComputeBuffer();
        //获取ComputeShader中对应方法索引
		int kernel = m_computeShader.FindKernel("CSMain");
		//设置数据buffer, 渲染的renderTexure 以及ComputeShader中StructedBuffer
		m_resultBuffer.SetData(m_inputData);
		m_computeShader.SetTexture(kernel, "Overdraw", m_overdrawTexture);
		m_computeShader.SetBuffer(kernel, "Output", m_resultBuffer);
        //每个线程组32x32线程，确保一个像素一个线程
		int xGroups = (m_overdrawTexture.width / 32); 
		int yGroups = (m_overdrawTexture.height / 32);
		//执行ComputeShader
		m_computeShader.Dispatch(kernel, xGroups, yGroups, 1);
        //从GPU取回数据
		m_resultBuffer.GetData(m_resultData);
		//从结果中获取数据来计算累加像素写入和累加overdraw
		TotalShadedFragments = 0;
		for (int i = 0; i < m_resultData.Length; i++)
		{
			TotalShadedFragments += m_resultData[i];
		}
		OverdrawRatio = (float)TotalShadedFragments / (xGroups * 32 * yGroups * 32);
		m_accumulatedIntervalFragments += TotalShadedFragments;
		m_accumulatedIntervalOverdraw += OverdrawRatio;
		m_intervalFrames ++;

		if (OverdrawRatio > MaxOverdraw) MaxOverdraw = OverdrawRatio;
	}
    #endregion
    private void RecreateCamera()
    {
        if(m_camera != null)
        {
            return;
        }
        m_camera = GetComponent<Camera>();
        if (m_camera == null)
        {
            m_camera = gameObject.AddComponent<Camera>();
        }
		m_camera.CopyFrom(Camera.main);
		m_camera.SetReplacementShader(m_replacementShader, null);
    }
	private void RecreateTexture()
	{
		if (m_overdrawTexture == null)
		{
			m_overdrawTexture = new RenderTexture(m_camera.pixelWidth, m_camera.pixelHeight, 24, RenderTextureFormat.ARGBFloat);
			m_overdrawTexture.enableRandomWrite = true;
			m_camera.targetTexture = m_overdrawTexture;
		}

		if (m_camera.pixelWidth != m_overdrawTexture.width || m_camera.pixelHeight != m_overdrawTexture.height)
		{
			m_overdrawTexture.Release();
			m_overdrawTexture.width = m_camera.pixelWidth;
			m_overdrawTexture.height = m_camera.pixelHeight;
		}
	}
    private void RecreateComputeShader()
    {
        if (m_computeShader != null)
        {
            return;
        }
        m_computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/DoubleGame/ComputeShaders/OverdrawParallelReduction.compute");
        
    }
	private void RecreateComputeBuffer()
	{
        if (m_resultBuffer != null)
        {
            return;
        }
		m_resultBuffer = new ComputeBuffer(m_resultData.Length, 4); //float 4个字节
	}
	public void StartSampling()
	{
		enabled = true;
		m_camera.enabled = true;
	}	
	public void StopSampling()
	{
		enabled = false;
		m_camera.enabled = false;
	}
	public void ResetSampling()
	{
		m_accumulatedIntervalOverdraw = 0;
		m_accumulatedIntervalFragments = 0;
		m_intervalTime = 0;
		m_intervalFrames = 0;
        MaxOverdraw = 0;
	}
}
#endif