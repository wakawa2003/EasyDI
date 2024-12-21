using System.Collections;
using System.Net.WebSockets;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace EasyDI.UnitTest
{

    public class TestDI
    {

        public static characterController characterController;
        public static gunController gunController;

        [UnityTest, SetupUnitTestEasyDI]
        public IEnumerator TestDIWithMainProblem()
        {
            yield return new WaitForSeconds(0.5f);

            Debug.Log($"-----------start check--------------");
       
            Debug.Log($"character health after decore: {(characterController as iHealth).Health}");
            Debug.Log($"character speed after decore: {characterController.Speed}");

            if (string.IsNullOrEmpty(characterController.stringInMethod))
                Debug.LogError("inject stringInMethod fail");
            else
                Debug.Log("inject stringInMethod complete!!");
                    

            if (string.IsNullOrEmpty(characterController.StringProperties1))
                Debug.LogError("inject StringProperties1 fail");
            else
                Debug.Log("inject StringProperties1 complete!!");


            if (string.IsNullOrEmpty(characterController.stringFieldTag1))
                Debug.LogError("inject fail stringFieldTag1");
            else
                Debug.Log("inject stringFieldTag1 complete!!");


            //for character
            if (characterController.StringProperties1 == projectInstaller.stringDefault)
                Debug.LogError("can't overide project context value!");
            else
                Debug.Log("inject overide project context value complete!!");

            if (characterController.stringFieldTag2 != sceneInstaller.stringInstallForTag2)
                Debug.LogError("can't overide project context value!");
            else
                Debug.Log("inject  overide project context value complete!!");

            if (characterController.stringInMethod == projectInstaller.stringDefault)
                Debug.LogError("can't overide project context value!");
            else
                Debug.Log("inject overide project context value complete!!");

            if (characterController.stringFieldSingletonHasTag != sceneInstaller.stringTagForTagSingleton)
                Debug.LogError("can't inject singleton!");
            else
                Debug.Log("inject inject singleton complete!!");

            if (characterController.intInmethod != characterInstaller.intInMethod)
                Debug.LogError("can't inject singleton!");
            else
                Debug.Log("inject inject singleton complete!!");

            //for gun
            if (string.IsNullOrEmpty(gunController.stringField))
                Debug.LogError("inject fail");
            else
                Debug.Log("inject gunController.stringField complete!!");

            if (gunController.characterOwner == null)
                Debug.LogError("inject fail");
            else
                Debug.Log("inject gunController.characterOwner complete!!");

            if (gunController.stringField == projectInstaller.stringDefault)
                Debug.LogError("can't overide project context value!");
            else
                Debug.Log("inject gunController.stringField complete!!");


            //yield return new WaitForSeconds(0.5f);

            //check singleton inject
            if (characterController.classIsSingleton != gunController.classIsSingleton)
                Debug.LogError("singleton inject fail!");
            else
                Debug.Log("inject singleton inject complete!!");


            if (sceneInstaller.buffSpeedValue + characterInstaller.buffSpeedValue1 * 2 + characterInstaller.buffSpeedValue2 != characterController.Speed)
                Debug.LogError("decorator buffSpeed fail!!!");
            else
                Debug.Log("inject decorator buffSpeed complete!!");


            if ((characterController as iHealth).Health != 15)
                Debug.LogError("decorator health fail!!!");
            else
                Debug.Log("inject decorator Health complete!!");

            Debug.Log($"--------------end check--------------");
        }



        public class SetupUnitTestEasyDIAttribute : NUnitAttribute, IOuterUnityTestAction
        {
            public IEnumerator BeforeTest(ITest test)
            {
                yield return setupProjectContext();
                yield return setupSceneContext();
                yield return setupGameObject();
                yield return setupIngameControllerTest();
                yield return new EnterPlayMode();
            }

            public IEnumerator AfterTest(ITest test)
            {
                Debug.Log($"End test!!");
                yield return new ExitPlayMode();
            }

            IEnumerator setupProjectContext()
            {
                Scene scene = SceneManager.CreateScene($"Scene test {SceneManager.loadedSceneCount + 1}");
                SceneManager.SetActiveScene(scene);
                Debug.Log($"-setup Project Context");
                var projectCOntext = new GameObject("Project context");
                projectCOntext.gameObject.AddComponent<projectInstaller>();
                projectCOntext.gameObject.AddComponent<ProjectContext>();
                var p = ProjectContext.Ins;
                //LogAssert.Expect(LogType.Log, "Before Test!!");
                if (projectCOntext.gameObject != p.gameObject)
                    LogAssert.Expect(LogType.Error, "Khoi tao PJ Context fail!!");
                yield return null;
            }

            IEnumerator setupSceneContext()
            {
                Debug.Log($"--setup Scene Context");
                var obj = new GameObject("Scene context");
                obj.gameObject.AddComponent<sceneInstaller>();
                obj.gameObject.AddComponent<SceneContext>();

                yield return null;
            }

            IEnumerator setupGameObject()
            {
                Debug.Log($"---setup GO");
                var character = new GameObject("character Test 1");
                character.gameObject.AddComponent<characterInstaller>();
                TestDI.characterController = character.gameObject.AddComponent<characterController>();
                character.gameObject.AddComponent<GameObjectContext>();



                Debug.Log($"---setup GUn");
                var gun = new GameObject("Gun Test 1");
                gun.transform.SetParent(character.transform);
                gun.gameObject.AddComponent<gunInstaller>();
                TestDI.gunController = gun.gameObject.AddComponent<gunController>();
                gun.gameObject.AddComponent<GameObjectContext>();

                yield return null;
            }

            IEnumerator setupIngameControllerTest()
            {
                Debug.Log($"setup ingameControllerTest");
                var obj = new GameObject("ingame Controller");
                obj.gameObject.AddComponent<ingameControllerTest>();
                yield return null;
            }
        }
    }

}