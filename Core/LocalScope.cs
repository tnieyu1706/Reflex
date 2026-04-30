using Reflex.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Core
{
    // Chạy sau Scene Container một chút, nhưng trước tất cả các Injector và Awake thông thường
    [DefaultExecutionOrder(ContainerScope.SceneContainerScopeExecutionOrder + 10)]
    public class LocalScope : MonoBehaviour
    {
        public Container SelfContainer { get; private set; }

        private void Awake()
        {
            // 1. Tìm Scope cha gần nhất trên Transform hierarchy. 
            // Nếu không có parent transform nào có GameObjectScope, lấy Scene Container làm gốc.
            var parentContainer = gameObject.TryGetClosestLocalContainer(out var localContainer)
                ? localContainer
                : gameObject.scene.GetSceneContainer();

            // 2. Tạo một Scope mới kế thừa từ Parent Container
            SelfContainer = parentContainer.Scope(builder =>
            {
                builder.SetName($"GameObjectScope ({gameObject.name})");

                // 3. Lấy tất cả Installers gắn trên *chính* GameObject này để cài đặt bindings
                using (ListPool<IInstaller>.Get(out var installers))
                {
                    GetComponents(installers);
                    for (var i = 0; i < installers.Count; i++)
                    {
                        installers[i].InstallBindings(builder);
                    }
                }
            });
        }

        private void OnDestroy()
        {
            SelfContainer?.Dispose();
        }
    }
}