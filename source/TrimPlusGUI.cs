using System;
using UnityEngine;
using KSP.UI.Screens;

using static TrimPlus.RegisterToolbar;
using ToolbarControl_NS;
using ClickThroughFix;
using SpaceTuxUtility;


namespace TrimPlus
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class TrimPlusSettings : MonoBehaviour
    {
        internal const string MODID = "TrimPlus";
        internal const string MODNAME = "TrimPlus";

        static ToolbarControl toolbarControl;

        KeyBinding listeningFor = null;
        bool listeningForPrimary;
        
        static bool likeAVirgin = true;

        int enterExitWinId;

        public void Awake()
        {
            if (likeAVirgin)
            {
                TrimPlus.LoadFromConfig();
                likeAVirgin = false;
            }
            TrimPlus.SetDefaultBindings();
        }

        void Start()
        {
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(this.ToggleMainWindow, this.ToggleMainWindow,
                        ApplicationLauncher.AppScenes.SPACECENTER,
                    MODID,
                    "TrimPlusButton",
                    "TrimPlus/PluginData/TrimPlus",
                    "TrimPlus/PluginData/TrimPlus",
                    MODNAME
                );
            }
            enterExitWinId = WindowHelper.NextWindowId("FRSWin");

        }
        bool Active = false;
        void ToggleMainWindow() { this.Active = !this.Active; }

        public void Update()
        {
            if(listeningFor != null)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    listeningFor = null;
                    return;
                }

                for (int i = 0; i <= 429; i++)
                {
                    try
                    {
                        KeyCode code = (KeyCode)i;
                        if(Input.GetKeyDown(code))
                        {
                            if (listeningForPrimary)
                            {
                                listeningFor.primary = new KeyCodeExtended(code);
                                listeningFor = null;
                                break;
                            }
                            else
                            {
                                listeningFor.secondary = new KeyCodeExtended(code);
                                listeningFor = null;
                                break;
                            }
                        }
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                    
                }   

            }
        }


        Rect windowRect = new Rect(0, 0, 600, 50);

        public void OnGUI()
        {
            GUI.skin = HighLogic.Skin;
            if (Active)
            windowRect = ClickThruBlocker.GUILayoutWindow(enterExitWinId, windowRect, windowFunc, "TRIM PLUS", GUILayout.Width(600f));
            //windowRect.height = 0f;
        }

        public void windowFunc(int id)
        {
            GUILayout.Space(3);
            
            {
                for (int i = 0; i < (int)BindingName.COUNT; i++)
                {
                    KeyBinding binding = TrimPlus.Bindings[i];

                    if (listeningFor != null)
                        GUI.enabled = false;
                    else
                        GUI.enabled = true;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(((BindingName)i).ToString(), GUILayout.Width(150f));

                    if (GUILayout.Button(listeningFor == binding && listeningForPrimary ? "press key..." : binding.primary.ToString()))
                    {
                        listeningFor = binding;
                        listeningForPrimary = true;
                    }

                    if (GUILayout.Button(listeningFor == binding && !listeningForPrimary ? "press key..." : binding.secondary.ToString()))
                    {
                        listeningFor = binding;
                        listeningForPrimary = false;
                    }

                    if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                    {
                        binding.primary = new KeyCodeExtended(KeyCode.None);
                        binding.secondary = new KeyCodeExtended(KeyCode.None);
                    }

                    GUILayout.EndHorizontal();

                    GUI.enabled = true;

                }

                if (GUILayout.Button("save"))
                    TrimPlus.SaveToConfig();

            }
            GUI.DragWindow();       
        }

    }
}
