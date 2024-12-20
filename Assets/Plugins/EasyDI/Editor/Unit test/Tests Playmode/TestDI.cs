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

            if (string.IsNullOrEmpty(characterController.stringInMethod))
                Debug.LogError("inject fail");
            if (string.IsNullOrEmpty(characterController.StringProperties1))
                Debug.LogError("inject fail");
            if (string.IsNullOrEmpty(characterController.stringFieldTag1))
                Debug.LogError("inject fail");

            //for character
            if (characterController.StringProperties1 == projectInstaller.stringDefault)
                Debug.LogError("can't overide project context value!");
            if (characterController.stringFieldTag2 != sceneInstaller.stringInstallForTag2)
                Debug.LogError("can't overide project context value!");
            if (characterController.stringInMethod == projectInstaller.stringDefault)
                Debug.LogError("can't overide project context value!");
            if (characterController.stringFieldSingletonHasTag != sceneInstaller.stringTagForTagSingleton)
                Debug.LogError("can't inject singleton!");
            if (characterController.intInmethod != characterInstaller.intInMethod)
                Debug.LogError("can't inject singleton!");
            //for gun
            if (string.IsNullOrEmpty(gunController.stringField))
                Debug.LogError("inject fail");
            if (gunController.characterOwner == null)
                Debug.LogError("inject fail");
            if (gunController.stringField == projectInstaller.stringDefault)
                Debug.LogError("can't overide project context value!");

            yield return new WaitForSeconds(0.5f);
            Debug.Log($"character speed after decore: {characterController.Speed}");
            //check singleton inject
            if (characterController.classIsSingleton != gunController.classIsSingleton)
                Debug.LogError("singleton inject fail!");

            if (sceneInstaller.buffSpeedValue + characterInstaller.buffSpeedValue1 * 2 + characterInstaller.buffSpeedValue2 != characterController.Speed)
                Debug.LogError("decorator fail!!!");

            //var listDecore = (characterController as IEasyDIDecore<iSpeed>).ToListDecore();
            //if (listDecore.Count != 5)
            //    Debug.LogError("count decorator fail!!!");


            //Debug.Log($"Decore list:");
            //int i = 0;
            //(characterController as IEasyDIDecore<iSpeed>).ForeachDecore(_ =>
            //{
            //    Debug.Log($"   {i++}: {_}");
            //});



            yield return new WaitForSeconds(1f);
            yield return null;
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
                Debug.Log($"setup Project Context");
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
                Debug.Log($"setup Scene Context");
                var obj = new GameObject("Scene context");
                obj.gameObject.AddComponent<sceneInstaller>();
                obj.gameObject.AddComponent<SceneContext>();

                yield return null;
            }

            IEnumerator setupGameObject()
            {
                Debug.Log($"setup GO");
                var character = new GameObject("character Test 1");
                character.gameObject.AddComponent<characterInstaller>();
                TestDI.characterController = character.gameObject.AddComponent<characterController>();
                character.gameObject.AddComponent<GameObjectContext>();



                Debug.Log($"setup GUn");
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