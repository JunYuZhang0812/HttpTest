#if USE_UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using Network;
using Network.Messages;
using GwScene;

public static class GameInfoCollector
{
    public static string curSceneName = string.Empty;

    #region 协议信息统计
    public class MessageDataInfo
    {
        public int messageID;
        public DateTime time;
        public int dataSize;
    };
    public class MessageInfo
    {
        public List<MessageDataInfo> messageDataInfo = new List<MessageDataInfo>();
        public string messageName = string.Empty;
        public string extInfo = string.Empty;
        public int id = 0;
        public bool fold = false;
        
        private long totalBytes = 0;

        public long TotalBytes 
        {
            get 
            {
                return totalBytes;
            }
        }

        public int Count 
        {
            get 
            {
                return messageDataInfo.Count;
            }
        }

        public void AddMessageData(MessageData data)
        {
            var info = new MessageDataInfo();
            info.time = DateTime.Now;
            info.dataSize = data.dataSize;
            info.messageID = data.id;
            totalBytes += (data.dataSize + 4);
            messageDataInfo.Add(info);
        }

        public void AddMessageData(int id, int dataSize)
        {
            var info = new MessageDataInfo();
            info.time = DateTime.Now;
            info.dataSize = dataSize;
            info.messageID = id;
            totalBytes += (dataSize + 4);
            messageDataInfo.Add(info);
        }
        public void AddMessageData(BaseMessage data)
        {
            var info = new MessageDataInfo();
            info.time = DateTime.Now;
            info.messageID = data.GetId();
            messageDataInfo.Add(info);
        }

        public string MessageType()
        {
            return string.Format("Id:{0}-{1}:\t{2}\t[{3}] {4} {5}",id, messageName, messageDataInfo.Count, messageDataInfo[Count - 1].time.ToLongTimeString(), extInfo, EditorUtility.FormatBytes(totalBytes));
        }
        public void ContentsGUI(bool containSize = true)
        {
            messageDataInfo.ForEach( dataInfo => 
            {
                if(containSize)
                {
                    EditorGUILayout.TextField(string.Format("[{0}], dataSize:{1}",dataInfo.time.ToLongTimeString(), dataInfo.dataSize),  GUILayout.ExpandWidth(true));
                }
                else 
                {
                    EditorGUILayout.TextField(string.Format("[{0}], extInfo: {1}",dataInfo.time.ToLongTimeString(), extInfo), GUILayout.ExpandWidth(true));
                }
            });
        }
    }
    public class IgnoreMessageInfo
    {
        public enum IgnoreType
        {
            Inexistence = 0,
            PlaceHolderModel = 1
        }

        public void AddMessageData(BaseMessage message, string extInfo)
        {
            var id = message.GetId();
            MessageInfo info = null;
            if(!messageDataInfo.TryGetValue(id, out info))
            {
                info = new MessageInfo();
                info.id = id;
                info.messageName = message.ToString();
                info.extInfo = extInfo;
                messageDataInfo.Add(id, info);
            }
            info.AddMessageData(message);
        }

        public int ignoreCount 
        {
            get 
            {
                var count = 0;
                var iter = messageDataInfo.GetEnumerator();
                while(iter.MoveNext())
                {
                    count += iter.Current.Value.messageDataInfo.Count;
                }
                return count;
            }
        }
        public void ContentsGUI()
        {
            var iter = messageDataInfo.GetEnumerator();
            while(iter.MoveNext())
            {
                var info = iter.Current.Value;
                info.fold = EditorGUILayout.Foldout(info.fold, info.MessageType());
                EditorGUI.indentLevel ++;
                if(info.fold)
                {
                    info.ContentsGUI(false);
                }
                EditorGUI.indentLevel --;
            }
        }
        public IgnoreType type = IgnoreType.Inexistence;
        public Dictionary<int, MessageInfo> messageDataInfo = new Dictionary<int, MessageInfo>();
        public UInt64 uid = 0;
        public bool fold = false;
    }

    public static Dictionary<int , MessageInfo> dicReceiveMessageInfos = new Dictionary<int, MessageInfo>();
    public static Dictionary<int , MessageInfo> dicSendMessageInfos = new Dictionary<int, MessageInfo>();
    public static Dictionary<UInt64 , IgnoreMessageInfo> dicIgnoreInexistenceMessageInfos = new Dictionary<UInt64, IgnoreMessageInfo>();
    public static Dictionary<UInt64 , IgnoreMessageInfo> dicIgnorePlaceHolderMessageInfos = new Dictionary<UInt64, IgnoreMessageInfo>();
    public static long totalSocketReceiveSize = 0;
    public static void CollectReceiveMessage(MessageData data, Common.FastBinaryReader reader)
    {
        MessageInfo info = null;
        if(dicReceiveMessageInfos.TryGetValue(data.id, out info))
        {
            info.AddMessageData(data);
        }
        else 
        {
            var message = MessageFactory.Create( data.id, reader );
            if(message != null)
            {
                info = new MessageInfo();
                info.id = data.id;
                info.messageName = message.ToString();
                info.AddMessageData(data);
                dicReceiveMessageInfos.Add(data.id, info);
            }
        }
    }

    public static void CollectReceiveMessage(int id, string name, int size)
    {
        MessageInfo info = null;
        if(dicReceiveMessageInfos.TryGetValue(id, out info))
        {
            info.AddMessageData(id, size);
        }
        else 
        {
            info = new MessageInfo();
            info.id = id;
            info.messageName = name;
            info.AddMessageData(id, size);
            dicReceiveMessageInfos.Add(id, info);
        }
    }

    public static void CollectSendMessage(int id, BaseMessage message, int size)
    {
        MessageInfo info = null;
        if(dicSendMessageInfos.TryGetValue(id, out info))
        {
            info.AddMessageData(id, size);
        }
        else 
        {
            info = new MessageInfo();
            info.id = id;
            info.messageName = message.ToString();
            info.AddMessageData(id, size);
            dicSendMessageInfos.Add(id, info);
        }
    }

    public static void CollectSendMessage(int id, int size)
    {
        MessageInfo info = null;
        if(dicSendMessageInfos.TryGetValue(id, out info))
        {
            info.AddMessageData(id, size);
        }
        else 
        {
            info = new MessageInfo();
            info.id = id;
            info.messageName = "LusMessage";
            info.AddMessageData(id, size);
            dicSendMessageInfos.Add(id, info);
        }
    }

    public static void CollectIgnoreInexistenceMessage(UInt64 characterUid, BaseMessage message)
    {
        IgnoreMessageInfo info = null;
        if(!dicIgnoreInexistenceMessageInfos.TryGetValue(characterUid, out info))
        {
            info = new IgnoreMessageInfo();
            info.uid = characterUid;
            info.type = IgnoreMessageInfo.IgnoreType.Inexistence;
            dicIgnoreInexistenceMessageInfos.Add(characterUid, info);
        }
        info.AddMessageData(message, string.Empty);
    }

    public static void CollectPlaceHolderMessage(UInt64 characterUid, BaseMessage message, string extInfo)
    {
        IgnoreMessageInfo info = null;
        if(!dicIgnorePlaceHolderMessageInfos.TryGetValue(characterUid, out info))
        {
            info = new IgnoreMessageInfo();
            info.uid = characterUid;
            info.type = IgnoreMessageInfo.IgnoreType.Inexistence;
            dicIgnorePlaceHolderMessageInfos.Add(characterUid, info);
        }
        info.AddMessageData(message, extInfo);
    }

    public static int RecieveMessageCount 
    {
        get 
        { 
            var conut = 0;
            var iter = dicReceiveMessageInfos.GetEnumerator();
            while(iter.MoveNext())
            {
                conut += iter.Current.Value.Count;
            }
            return conut;
        }
    }

    public static long RecieveMessageTotalSize
    {
        get 
        { 
            var totalBytes = 0l;
            var iter = dicReceiveMessageInfos.GetEnumerator();
            while(iter.MoveNext())
            {
                totalBytes += iter.Current.Value.TotalBytes;
            }
            return totalBytes;
        }
    }

    public static int SendMessageCount 
    {
        get 
        { 
            var conut = 0;
            var iter = dicSendMessageInfos.GetEnumerator();
            while(iter.MoveNext())
            {
                conut += iter.Current.Value.Count;
            }
            return conut;
        }
    }

    public static int IgnoreInexistenceMessageCount
    {
        get 
        { 
            var conut = 0;
            var iter = dicIgnoreInexistenceMessageInfos.GetEnumerator();
            while(iter.MoveNext())
            {
                conut += iter.Current.Value.ignoreCount;
            }
            return conut;
        }
    }

    public static int IgnorePlaceHolderMessageCount
    {
        get 
        { 
            var conut = 0;
            var iter = dicIgnorePlaceHolderMessageInfos.GetEnumerator();
            while(iter.MoveNext())
            {
                conut += iter.Current.Value.ignoreCount;
            }
            return conut;
        }
    }

    #endregion

    #region 场景特效信息
    public static List<ParticleSystem> curSceneParticleSystems = new List<ParticleSystem>();
    public static int SceneParticleSystemsCount 
    {
        get 
        {
            var entityVfxsCount = curSceneParticleSystems.Count;
            if(entityVfxsCount > 0)
            {
                var cout = 0;
                for(int i = 0; i < entityVfxsCount; ++i)
                {
                    var ps = curSceneParticleSystems[i];
                    if(ps == null || ps.isPaused || ps.isStopped)
                    {
                        continue;
                    }

                    if(ps.gameObject.activeSelf)
                    {
                        cout += ps.particleCount;
                    }
                }
                return cout;
            }
            return 0;
        }
    }

    
    public static void RefreshSceneVFX(bool force = false)
    {
        var atcivedScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(string.IsNullOrEmpty(curSceneName) || atcivedScene.name != curSceneName || force)
        {
            curSceneName = atcivedScene.name;
            var particles = GameObject.FindObjectsOfType(typeof(ParticleSystem));
            if(particles != null)
            {
                curSceneParticleSystems.Clear();
                for(int i = 0; i < particles.Length; ++i)
                {   
                    var particle = particles[i] as ParticleSystem;
                    if (particle != null)
                    {
                        curSceneParticleSystems.Add(particle);
                    }
                }
            }
        }
    }
    #endregion

    #region 场景面数
    public class SceneMeshInfo
    {
        public MeshFilter meshFilter = null;
        public Math3d.AABB bounds = null;
        public bool toRender = false;
    }
    public class CharacterMeshInfo
    {
        public MeshFilter[] meshFilter = null;
        public SkinnedMeshRenderer[] skinMeshRender = null;
        public Transform tranform = null;
        public bool hasShadow = false;
        public int tris = 0;
        public bool toRender = false;
        public bool fold = false;
        public void Update(Math3d.ViewFrustum viewFrustum)
        {
            if(skinMeshRender != null)
            {
                Array.ForEach(skinMeshRender, render =>
                { 
                    render.sharedMesh.RecalculateBounds();
                    var blounds = render.sharedMesh.bounds;
                    blounds.center = render.transform.localToWorldMatrix.MultiplyPoint(blounds.center);
                    var aabb = new Math3d.AABB(blounds.min, blounds.max);
                    toRender |= viewFrustum.IntersectsWithoutBoxTest(aabb);
                });
                tris = 0;
                if(toRender)
                {
                     Array.ForEach(skinMeshRender, render=>
                     { 
                        if(render.enabled)
                        {
                            tris += render.sharedMesh.triangles.Length / 3;
                        }
                     });
                     Array.ForEach(meshFilter, meshFilter=>
                     { 
                        if(meshFilter.gameObject.activeInHierarchy)
                        {
                            tris += meshFilter.sharedMesh.triangles.Length / 3;
                        }
                     });
                }
            }
        }

        public void InfoGUI()
        {
            EditorGUI.indentLevel ++;
            Array.ForEach(skinMeshRender, render=>
            { 
                if(render.enabled)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.ObjectField(render.gameObject, typeof(GameObject), false, GUILayout.ExpandWidth(true));
                        GUILayout.Label("面数：" + render.sharedMesh.triangles.Length / 3, GUILayout.Width(100));
                    }
                    GUILayout.EndHorizontal();
                }
            });
            Array.ForEach(meshFilter, meshFilter=>
            { 
                if(meshFilter.gameObject.activeInHierarchy)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.ObjectField(meshFilter.gameObject, typeof(GameObject), false, GUILayout.ExpandWidth(true));
                        GUILayout.Label("面数：" + meshFilter.sharedMesh.triangles.Length / 3, GUILayout.Width(100));
                    }
                    GUILayout.EndHorizontal();
                }
            });
            EditorGUI.indentLevel --;
        }
    }

    public static List<SceneMeshInfo> curMeshInfos = new List<SceneMeshInfo>();
    public static List<CharacterMeshInfo> curCharacterMeshInfos = new List<CharacterMeshInfo>();
    public static int sceneRenderTriangles = 0;
    public static int characterRenderTriangles = 0;
    public static void RefreshSceneInfo()
    {
        //场景Mesh
        curMeshInfos.Clear();
        var sceneLoader = SceneLoaderManager.Instance.GetCurSceneLoader(); 
        if(sceneLoader != null)
        {
            var meshRenders = sceneLoader.ListSceneMeshRenders;
            meshRenders.ForEach(meshRender => 
            { 
                if(meshRender != null)
                {
                    var sceneMeshInfo = new SceneMeshInfo();
                    var meshFilter = meshRender.GetComponent<MeshFilter>();
                    if(meshFilter != null)
                    {
                        meshFilter.sharedMesh.RecalculateBounds();
                        var blounds = meshFilter.sharedMesh.bounds;
                        sceneMeshInfo.meshFilter = meshFilter;
                        sceneMeshInfo.bounds = new Math3d.AABB(blounds.min, blounds.max);
                        curMeshInfos.Add(sceneMeshInfo);                  
                    }
                }
            });
        }
    }
    public static void RefreshSceneCharacterTris()
    {
        //角色相关Mesh
        var viewFrustum = CameraManager.Instance.GetViewFrustum();
        characterRenderTriangles = 0;
        curCharacterMeshInfos.Clear();
        var entitys = GameSceneManager.Instance.entities.GetAll();
        var localPlayer = GameSceneManager.Instance.localPlayer;
        if(entitys.Count > 0)
        {
            entitys.ForEach(entity => 
            { 
                if(entity.transform.gameObject.layer != LayerUtils.Hide)
                {
                    var meshInfo = new CharacterMeshInfo();
                    meshInfo.hasShadow = localPlayer == entity && !CombatUtils.IsMutilMap();
                    meshInfo.tranform = entity.gameObject.transform;
                    meshInfo.skinMeshRender = entity.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();;
                    meshInfo.meshFilter = entity.transform.GetComponentsInChildren<MeshFilter>();
                    meshInfo.Update(viewFrustum);
                    characterRenderTriangles += meshInfo.hasShadow ? meshInfo.tris * 2 : meshInfo.tris;
                    curCharacterMeshInfos.Add(meshInfo);
                }
            });
        }
    }
    public static void RefreshSceneTris()
    {
        sceneRenderTriangles = 0;
        var viewFrustum = CameraManager.Instance.GetViewFrustum();
        curMeshInfos.ForEach(meshInfo =>
        { 
            meshInfo.toRender = false;
            var obj = meshInfo.meshFilter.gameObject;
            if(obj.activeInHierarchy && obj.layer != LayerUtils.Hide)
            {
                meshInfo.toRender = viewFrustum.IntersectsWithoutBoxTest(meshInfo.bounds);
                if(meshInfo.toRender)
                {
                    sceneRenderTriangles += meshInfo.meshFilter.sharedMesh.triangles.Length / 3;
                }
            }
        });
    }
    #endregion

    #region 加载信息统计
    public class AssetInfo 
    {
        public string assetPath = string.Empty;
        public int assetId      = 0;
        public int playerLevel  = 0;
        public string WritePathInfo()
        {
            return string.Format("{0}|{1}", assetPath, playerLevel);
        }
        public string WriteIDInfo()
        {
            return string.Format("{0}|{1}", assetId, playerLevel);
        }

        public string PathInfo(string info = "")
        {
            if(string.IsNullOrEmpty(info))
            {
                return string.Format("{0}|{1}", assetPath, playerLevel);
            }

            return string.Format("{0}：{1}|{2}", info, assetPath, playerLevel);
        }

        public string IDInfo(string info = "")
        {
            if (string.IsNullOrEmpty(info))
            {
                return string.Format("{0}|{1}", assetId, playerLevel);
            }
            return string.Format("{0}：{1}|{2}", info, assetId, playerLevel);
        }
    }
    public class SceneAssetInfo
    {
        public bool foldout                                     = true;
        public string sceneInfo                                 = string.Empty;
        public Dictionary<string, AssetInfo> assetInfos         = new Dictionary<string, AssetInfo>();
        public Dictionary<int, AssetInfo> monsterIDs            = null;
        public Dictionary<string, AssetInfo> levelMapInfos      = null;
        public SceneAssetInfo(string info)
        {
            foldout         = true;
            sceneInfo       = info;
        }

        public void AddAsset(string path, int level)
        {
            AssetInfo assetInfo = null;
            if(!assetInfos.TryGetValue(path, out assetInfo))
            {
                assetInfo = new AssetInfo();
                assetInfo.assetPath     = path;
                assetInfo.playerLevel = level;
                assetInfos.Add(path, assetInfo);
            }
        }

        public void AddMonsterID(int monsterID, int level)
        {
            if(monsterIDs == null)
            {
                monsterIDs = new Dictionary<int, AssetInfo>();
            }
            AssetInfo assetInfo = null;
            if (!monsterIDs.TryGetValue(monsterID, out assetInfo))
            {
                assetInfo = new AssetInfo();
                assetInfo.assetId = monsterID;
                assetInfo.playerLevel = level;
                monsterIDs.Add(monsterID, assetInfo);
            }
        }

        public void AddLevelMapInfos(string levelMap, int level)
        {
            if(levelMapInfos == null)
            {
                levelMapInfos = new Dictionary<string, AssetInfo>();
            }
            AssetInfo assetInfo = null;
            if (!levelMapInfos.TryGetValue(levelMap, out assetInfo))
            {
                assetInfo = new AssetInfo();
                assetInfo.assetPath = levelMap;
                assetInfo.playerLevel = level;
                levelMapInfos.Add(levelMap, assetInfo);
            }
        }
    }


    public static SceneAssetInfo curSceneAssetInfo = null;
    public static Dictionary<string, SceneAssetInfo> resourceLoadRecords = new Dictionary<string, SceneAssetInfo>();
    public static List<string> loadFailAssetsRecords = new List<string>();
    public static List<string> loadUIPrefabsRecords = new List<string>();
    public static bool recordPlayerLevel = false;
    private static int PlayerLevel
    {
        get
        {
            var level = 0;
            if(recordPlayerLevel)
            {
                var lp = GameSceneManager.Instance.localPlayer;
                level = lp != null ? lp.Level : 0;
            }
            return level;
        }
    }

    public static void StartResourceRecord()
    {
        resourceLoadRecords.Clear();

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene == null)
        {
            curSceneAssetInfo = new SceneAssetInfo("Start-0");
        }
        else 
        {
            var sceneName = scene.name;
            var sceneInfo = string.Format("{0}-{1}",sceneName, PlayerLevel);
            curSceneAssetInfo = new SceneAssetInfo(sceneInfo);
        }

        resourceLoadRecords.Add(curSceneAssetInfo.sceneInfo, curSceneAssetInfo);
    }

    public static void EndResourceRecord()
    {
        if(resourceLoadRecords.Count > 0)
        {
            //记录加载的资源
            var recordFilePath = string.Format("Assets/OutPut/RecordedAssets/LoadAssets/LoadAssets_{0}.txt", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
            var recordFileStreamWriter = new StreamWriter(recordFilePath);
            //记录怪物ID
            var recordMonsterIDFilePath = string.Format("Assets/OutPut/RecordedAssets/MonsterIDs/MonsterID_{0}.txt", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
            var recordMonsterStreamWriter = new StreamWriter(recordMonsterIDFilePath);
            //关卡配置
            var recordLevelMapInfoFilePath = string.Format("Assets/OutPut/RecordedAssets/LevelMapInfos/LevelMapInfo_{0}.txt", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
            var levelMapInfoStreamWriter = new StreamWriter(recordLevelMapInfoFilePath);

            var iter = resourceLoadRecords.GetEnumerator();
            while(iter.MoveNext())
            {
                var sceneInfo       = iter.Current.Key;
                var sceneAssetInfo  = iter.Current.Value;

                recordFileStreamWriter.WriteLine(string.Format("--------------------{0}--------------------", sceneInfo));
                using(var assetIter = sceneAssetInfo.assetInfos.GetEnumerator())
                {
                    while(assetIter.MoveNext())
                    {
                        var info = assetIter.Current.Value;
                        recordFileStreamWriter.WriteLine(info.WritePathInfo());
                    }
                }

                if(sceneAssetInfo.monsterIDs != null)
                {
                    using (var assetIter = sceneAssetInfo.monsterIDs.GetEnumerator())
                    {
                        while (assetIter.MoveNext())
                        {
                            var info = assetIter.Current.Value;
                            recordMonsterStreamWriter.WriteLine(info.WriteIDInfo());
                        }
                    }
                }

                if(sceneAssetInfo.levelMapInfos != null)
                {
                    using (var assetIter = sceneAssetInfo.levelMapInfos.GetEnumerator())
                    {
                        while (assetIter.MoveNext())
                        {
                            var info = assetIter.Current.Value;
                            levelMapInfoStreamWriter.WriteLine(info.WritePathInfo());
                        }
                    }
                }
            }
            iter.Dispose();

            recordFileStreamWriter.Close();
            recordMonsterStreamWriter.Close();
            levelMapInfoStreamWriter.Close();

            EditorUtility.DisplayDialog("资源记录", "资源记录导出成功！请记得提交数据哦！\n\n数据路径：Assets/OutPut/RecordedAssets", "确定");
        }
    }

    public static void SwitchScene(string sceneName)
    {
        recordPlayerLevel = true;
        if (curSceneAssetInfo != null)
        {
            curSceneAssetInfo.foldout = false;
            curSceneAssetInfo = null;
        }

        var sceneInfo = string.Format("{0}-{1}",sceneName, PlayerLevel);
        if(!resourceLoadRecords.TryGetValue(sceneInfo, out curSceneAssetInfo))
        {
            curSceneAssetInfo = new SceneAssetInfo(sceneInfo);
            resourceLoadRecords.Add(sceneInfo, curSceneAssetInfo);
        }
    }

    public static void AddMonsterIDRecord(int monsterID)
    {
        if (curSceneAssetInfo != null)
        {
            curSceneAssetInfo.AddMonsterID(monsterID, PlayerLevel);
        }
    }

    public static void AddLevelMapInfoRecord(string levelMapInfo)
    {
        if (curSceneAssetInfo != null)
        {
            curSceneAssetInfo.AddLevelMapInfos(levelMapInfo, PlayerLevel);
        }
    }

    public static void AddResourceRecord(string resPath)
    {
        if(curSceneAssetInfo != null)
        {
            curSceneAssetInfo.AddAsset(resPath, PlayerLevel);
        }
    }

    public static void AddLoadFailResourceRecord(string fullPath)
    {
        if (loadFailAssetsRecords != null)
        {
            if(!loadFailAssetsRecords.Contains(fullPath))
            {
                loadFailAssetsRecords.Add(fullPath);
            }
        }
    }

    public static void AddLoadUIPrefabResourceRecord(string fullPath)
    {
        if (loadUIPrefabsRecords != null)
        {
            if (!loadUIPrefabsRecords.Contains(fullPath))
            {
                loadUIPrefabsRecords.Add(fullPath);
            }
        }
    }

    public static bool ExportResourceRecord(List<string> assetRecords, string recordFilePath)
    {
        if (assetRecords.Count > 0)
        {
            StreamWriter streamWriter = null;
            //var recordFilePath = recordFilePath;//
            try
            {
                if (File.Exists(recordFilePath))
                {
                    File.Delete(recordFilePath);
                }
                streamWriter = new StreamWriter(recordFilePath);
                streamWriter.WriteLine(assetRecords.Count);
                for (int i = 0; i < assetRecords.Count; ++i)
                {
                    streamWriter.WriteLine(assetRecords[i]);
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }
            }
            return true;
        }
        return false;
    }

    public static void RestoreUIPrefab(string fileName)
    {
        var resoucesUIFiles = Common.FileUtils.GetFileList("Assets/GameAssets/UI/Prefabs", file => { return file.EndsWith(".meta"); });
        var totalCount = resoucesUIFiles.Count;
        var restoreSuccess = false;
        var uiFileDestPath = string.Empty;
        for(int i = 0; i < resoucesUIFiles.Count; ++i)
        {
            var uiFilePath = resoucesUIFiles[i];
            var uiFileName = Path.GetFileNameWithoutExtension(uiFilePath);
            if(uiFileName.Equals(fileName))
            {
                uiFileDestPath = uiFilePath.Replace("GameAssets", "Resources");
                if(File.Exists(uiFileDestPath))
                {
                    return;
                }
                EditorUtility.DisplayProgressBar("UI预制还原中....", resoucesUIFiles[i], i * 1.0f / totalCount);
                var dir = Path.GetDirectoryName(uiFileDestPath);
                Directory.CreateDirectory(dir);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                restoreSuccess  = AssetDatabase.CopyAsset(uiFilePath, uiFileDestPath);
            }
        }

        if(restoreSuccess)
        {
            EditorUtility.DisplayDialog("还原UI预制成功！", uiFileDestPath + "\n\n亲，请立刻截图丢D3大群里告知肖肖哥哥哦！\n\n并让程序重新导出UI预制信息", "确认");
        }
        else 
        {
            EditorUtility.DisplayDialog("还原UI预制失败！", "未找到UI预制：" + fileName, "确认");
        }
        EditorUtility.ClearProgressBar();
    }
    #endregion

    #region CreateBundle统计
    public static List<string> bundleInfos = new List<string>();
    public static void AddBundleInfo(string info)
    {
        if(!bundleInfos.Contains(info))
        {
            bundleInfos.Add(info);
        }
    }
    public static void ClearBundleInfo()
    {
        bundleInfos.Clear();
    }
    #endregion

    #region Bundle Load统计
    public static List<string> bundleLoadInfos = new List<string>();
    public static void AddBundleLoadInfo(string loadInfo)
    {
        if(!bundleLoadInfos.Contains(loadInfo))
        {
            bundleLoadInfos.Add(loadInfo);
        }
    }
    public static void ClearBundleLoadInfo()
    {
        bundleLoadInfos.Clear();
    }

    #endregion

    #region IO操作和读取相关统计
    private static int runtimeGameFrame = -1;
    public static int maxFileOpenCount = 0;
    public static int maxFileReadCount = 0;
    public class FileOpenAndReadCountInfo
    {
        public int runtimeBundleAssetLoadCount = 0;
        public int runtimeFileOpenCount = 0;
        public int runtimeMarshalReadCount = 0;
        public int marshalReadBytesLength = 0;
        public int gameFrame = 0;
        public List<string> fileNames = new List<string>();

        public void AddFile(string fileName)
        {
            fileNames.Add(fileName);
        }
    }

    public static List<FileOpenAndReadCountInfo> fileOpenAndReadCountInfoList = new List<FileOpenAndReadCountInfo>();
    public static void CollectBundleAssetLoadCount()
    {
        if (runtimeGameFrame != Time.frameCount)
        {
            if(fileOpenAndReadCountInfoList.Count > 0)
            {
                maxFileOpenCount = Math.Max(maxFileOpenCount, fileOpenAndReadCountInfoList[fileOpenAndReadCountInfoList.Count - 1].runtimeFileOpenCount);
                maxFileReadCount = Math.Max(maxFileReadCount, fileOpenAndReadCountInfoList[fileOpenAndReadCountInfoList.Count - 1].runtimeBundleAssetLoadCount);
            }
            fileOpenAndReadCountInfoList.Add(new FileOpenAndReadCountInfo());
            runtimeGameFrame = Time.frameCount;
        }
        var index = fileOpenAndReadCountInfoList.Count - 1;
        fileOpenAndReadCountInfoList[index].gameFrame = Time.frameCount;
        fileOpenAndReadCountInfoList[index].runtimeBundleAssetLoadCount ++;
    }

    public static void CollectFileOpenCount(string fileName)
    {
        if (runtimeGameFrame != Time.frameCount)
        {
            if (fileOpenAndReadCountInfoList.Count > 0)
            {
                maxFileOpenCount = Math.Max(maxFileOpenCount, fileOpenAndReadCountInfoList[fileOpenAndReadCountInfoList.Count - 1].runtimeFileOpenCount);
                maxFileReadCount = Math.Max(maxFileReadCount, fileOpenAndReadCountInfoList[fileOpenAndReadCountInfoList.Count - 1].runtimeBundleAssetLoadCount);
            }
            fileOpenAndReadCountInfoList.Add(new FileOpenAndReadCountInfo());
            runtimeGameFrame = Time.frameCount;
        }
        var index = fileOpenAndReadCountInfoList.Count - 1;
        fileOpenAndReadCountInfoList[index].gameFrame = Time.frameCount;
        fileOpenAndReadCountInfoList[index].runtimeFileOpenCount ++;
        fileOpenAndReadCountInfoList[index].AddFile(fileName);
    }

    public static void CollectMarshalReadCount(int byteCount)
    {
        if (runtimeGameFrame != Time.frameCount)
        {
            if (fileOpenAndReadCountInfoList.Count > 0)
            {
                maxFileOpenCount = Math.Max(maxFileOpenCount, fileOpenAndReadCountInfoList[fileOpenAndReadCountInfoList.Count - 1].runtimeFileOpenCount);
                maxFileReadCount = Math.Max(maxFileReadCount, fileOpenAndReadCountInfoList[fileOpenAndReadCountInfoList.Count - 1].runtimeBundleAssetLoadCount);
            }
            fileOpenAndReadCountInfoList.Add(new FileOpenAndReadCountInfo());
            runtimeGameFrame = Time.frameCount;
        }
        var index = fileOpenAndReadCountInfoList.Count - 1;
        fileOpenAndReadCountInfoList[index].gameFrame = Time.frameCount;
        fileOpenAndReadCountInfoList[index].runtimeMarshalReadCount++;
        fileOpenAndReadCountInfoList[index].marshalReadBytesLength += byteCount;
    }

    #endregion
}
#endif