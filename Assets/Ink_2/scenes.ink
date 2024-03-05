


LIST Scenes = MortuaryScene, MetroPlatformScene , BackRoomClubScene, AtTheCardTableScene

VAR levelDataFunction = -> sceneData

LIST SceneProps = Title, Date, Function, VOIntro

=== function sceneData(sceneID, prop) 
{sceneID: 
- MortuaryScene:   
{ prop:     
- VOIntro:  VO: Something's out of place.
- Title:    Mortuary, 4th Quartier 
- Date:     24th April 1968, 2:38pm
- Function: ~ return -> MortuaryScene_data
}
- MetroPlatformScene:   
{ prop:     
- VOIntro:  VO: Something belongs... elsewhere. In other hands.
- Title:    Metro Platform, Champ de Mars
- Date:     23rd April 1968, 11:25pm
- Function: ~ return -> MetroPlatformScene_data
}
}
