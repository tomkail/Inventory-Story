


LIST Scenes = 
OpeningStolenScene,
OpeningDeviceScene,
OpeningCaseScene, // <- start here!
GraveyardScene, 
MortuaryScene, 
MetroPlatformScene, 
BackRoomClubScene, 
AtTheCardTableScene,

DeviceActivatedScene

VAR levelDataFunction = -> sceneData

=== function sceneData(sceneID) 
{sceneID: 
- OpeningStolenScene:    ~ return -> OpeningStolenScene_data 
- OpeningDeviceScene:    ~ return -> OpeningDeviceScene_data 
- OpeningCaseScene:    ~ return     -> OpeningCaseScene_data 
- MortuaryScene:        ~ return    -> MortuaryScene_data   
- MetroPlatformScene:   ~ return    -> MetroPlatformScene_data
- GraveyardScene:       ~ return    -> GraveyardScene_data
- DeviceActivatedScene:     ~ return -> DeviceActivatedScene_data
}
~ return ()

