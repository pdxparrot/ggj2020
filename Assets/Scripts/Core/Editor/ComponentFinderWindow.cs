using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

using Spine.Unity;
using TMPro;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor
{
    public sealed class ComponentFinderWindow : Window.EditorWindow
    {
        private const string MainStyleSheet = "ComponentFinderWindow/Main";
        private const string WindowLayout = "ComponentFinderWindow/Window";

        private static readonly Type[] BuiltinComponents = {
            // audio
            typeof(AudioSource),

            // physics
            typeof(Rigidbody),

            // 2d physics
            typeof(Rigidbody2D),

            // colliders
            typeof(Collider),
            typeof(BoxCollider),
            typeof(CapsuleCollider),
            typeof(SphereCollider),
            typeof(MeshCollider),

            // 2d colliders
            typeof(Collider2D),

            // particles
            typeof(ParticleSystem),

            // ai
            typeof(NavMeshAgent),
            typeof(NavMeshObstacle),

            // ui
            typeof(TextMeshProUGUI),

            // spine
#if USE_SPINE
            typeof(SkeletonAnimation),
            typeof(SkeletonUtility),
#endif
        };

        private struct ComponentLookupResult
        {
            public GameObject Prefab;

            public int Count;
        }

        [MenuItem("PDX Party Parrot/Component Finder...")]
        public static void ShowWindow()
        {
            ComponentFinderWindow window = GetWindow<ComponentFinderWindow>();
            window.Show();
        }

        public override string Title => "Component Finder";

        private readonly List<Type> _componentTypes = new List<Type>();
        private readonly List<string> _componentNames = new List<string>();
        private readonly List<string> _filteredComponentNames = new List<string>();

        private readonly List<ComponentLookupResult> _selectedPrefabs = new List<ComponentLookupResult>();

        [CanBeNull]
        private Type SelectedComponentType => _componentTypes[_componentTypePopup.index];

        private TextField _filter;

        private PopupField<string> _componentTypePopup;

        private VisualElement _selectedPrefabsContainer;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _componentTypes.Add(null);
            foreach(Type t in BuiltinComponents) {
                _componentTypes.Add(t);
            }
            ReflectionUtils.FindSubClassesOfInNamespace<Component>(_componentTypes, EditorSettings.projectGenerationRootNamespace);

            foreach(Type t in _componentTypes) {
                if(null == t) {
                    _componentNames.Add("None");
                    continue;
                }

                _componentNames.Add(t.Namespace?.StartsWith(EditorSettings.projectGenerationRootNamespace) ?? false ? t.FullName : t.Name);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(rootVisualElement);

            _filter = rootVisualElement.Q<TextField>("component-filter");
            _filter.RegisterValueChangedCallback(FilterChangedEventHandler);

            // TODO: remove the filter for now, we likely want to change
            // from using a PopupField to a ListView so that filtering actually works
            _filter.parent.Remove(_filter);

            VisualElement componentTypesContainer = rootVisualElement.Q<VisualElement>("container-component-type");
            _componentTypePopup = new PopupField<string>("Component Type:", _componentNames, 0);
            _componentTypePopup.RegisterValueChangedCallback(OnComponentTypeChanged);
            componentTypesContainer.Add(_componentTypePopup);

            _selectedPrefabsContainer = rootVisualElement.Q<VisualElement>("container-selected-prefabs");
        }
#endregion

        // TODO: this could be done much better
        private void Filter()
        {
            _filteredComponentNames.Clear();

            _filteredComponentNames.AddRange(_componentNames.Where(x => {
                if(string.IsNullOrWhiteSpace(x)) {
                    return false;
                }

                // always include the None entry
                if("None" == x) {
                    return true;
                }

                return -1 != x.IndexOf(_filter.text, StringComparison.InvariantCultureIgnoreCase);
            }));

            // TODO: set the popup to the filtered types
        }

        private void UpdateSelectedPrefabs(Type selectedType)
        {
            _selectedPrefabs.Clear();

            Debug.Log($"Finding prefabs of type {selectedType}...");

            string[] assetGUIDs = AssetDatabase.FindAssets("t:prefab");
            foreach(string assetGUID in assetGUIDs) {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if(null == prefab) {
                    Debug.LogWarning($"AssetDatabase returned non prefab at {assetPath}");
                    continue;
                }

                var components = prefab.GetComponentsInChildren(selectedType);
                if(components.Length < 1) {
                    continue;
                }

                _selectedPrefabs.Add(new ComponentLookupResult {
                    Prefab = prefab,
                    Count = components.Length
                });
            }
        }

        private void UpdateSelectedPrefabsUI(Type selectedType)
        {
            _selectedPrefabsContainer.Clear();

            if(_selectedPrefabs.Count < 1) {
                _selectedPrefabsContainer.Add(new Label($"No prefabs of type {selectedType} found"));
                return;
            }

            foreach(ComponentLookupResult result in _selectedPrefabs) {
                VisualElement container = new VisualElement();
                container.AddToClassList("container-row");

                Button button = new Button {
                    text = result.Prefab.name
                };
                button.AddToClassList("core-button");
                button.clickable.clicked += () => {
                    Selection.activeGameObject = result.Prefab;
                    // TODO: how do we open the prefab?
                };

                container.Add(button);
                container.Add(new Label($"{AssetDatabase.GetAssetPath(result.Prefab)} has {result.Count} instances"));

                _selectedPrefabsContainer.Add(container);
            }
        }

#region Event Handlers
        private void OnComponentTypeChanged(ChangeEvent<string> evt)
        {
            Type selectedType = SelectedComponentType;
            if(null == selectedType) {
                return;
            }

            UpdateSelectedPrefabs(selectedType);

            UpdateSelectedPrefabsUI(selectedType);
        }

        private void FilterChangedEventHandler(ChangeEvent<string> evt)
        {
            Filter();
        }
#endregion
    }
}
