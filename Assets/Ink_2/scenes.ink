


LIST Scenes = MortuaryScene, MetroPlatformScene , BackRoomClubScene, AtTheCardTableScene

VAR levelDataFunction = -> sceneData

=== function sceneData(sceneID, prop) 
{sceneID: 
- MortuaryScene:        ~ return -> MortuaryScene_data   
- MetroPlatformScene:   ~ return -> MetroPlatformScene_data
}
~ return ()
