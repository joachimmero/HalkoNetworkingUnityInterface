using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HalkoNetworking
{
    public class TestAbility2 : RemoteMethod.HalkoClass
    {
        [RemoteMethod.HalkoMethod]
        private void TeleportToRandomLocationWithinRadiusr(int radius)
        {
            float x = Random.Range(transform.position.x - radius, transform.position.x + radius);
            float z = Random.Range(transform.position.z - radius, transform.position.z + radius);

            transform.position = new Vector3(x, transform.position.y, z);
        }

        [RemoteMethod.HalkoMethod]
        private void Test()
        {

        }
        [RemoteMethod.HalkoMethod]
        private void Test1()
        {

        }
    }
}

