


LIST Scenes = MetroPlatformScene , PlatformMurderScene

VAR levelDataFunction = -> sceneData

LIST SceneProps = Title, Date, Function, VOIntro

=== function sceneData(sceneID, prop) 
{sceneID: 
- MetroPlatformScene:   
{ prop:     
- VOIntro:  VO: Something belongs... elsewhere. In other hands.
- Title:    Metro Platform, Champ de Mars
- Date:     23rd April 1968, 11:25pm
- Function: ~ return -> MetroPlatformScene_data
}
}
