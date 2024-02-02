INCLUDE scenes.ink
INCLUDE sceneindex.ink
INCLUDE itemdata.ink
INCLUDE backend/system.ink
INCLUDE backend/aux.ink
INCLUDE scenedata.ink
INCLUDE templates.ink
INCLUDE scenes_timeline2.ink



[ Scene Count: {LIST_COUNT(LIST_ALL(Scenes))} ]

VAR LoopCount = 0

VAR DEBUG = false 

VAR previousSceneID = ()
VAR currentSceneID = ()

VAR TopSceneID = Pinboard

-> proceedTo(ParkingLot) 


-> proceedTo(OpeningSequence)





