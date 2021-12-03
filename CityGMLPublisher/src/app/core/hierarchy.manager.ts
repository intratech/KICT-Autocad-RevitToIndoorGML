import { Model } from "./models/DTO";
import { WallManager } from "./wall.manager";
import { Config } from "../config";
import { CeilingAndFloorManager } from "./ceiling-floor-manager";

export class HierarchyManager {
  private wallManager: WallManager;
  private ceilingAndfloor: CeilingAndFloorManager
  constructor(wallManager: WallManager, ceilingAndfloor: CeilingAndFloorManager) {
    this.wallManager = wallManager;
    this.ceilingAndfloor = ceilingAndfloor;
  }

  public loadHierarchy(model: Model): Promise<any> {
    return new Promise((resolve, reject) => {
      let hierarchy = [];

      for (let level of model.levels) {
        let levelNode = {
          id: level.id,
          name: level.name,
          type: Config.LEVEL_TYPE,
          children: []
        }

        for (let room of model.rooms) {
          if (room.level_id != level.id)
            continue;

          let roomNode = {
            id: room.id,
            name: room.name,
            type: Config.ROOM_TYPE,
            properties: room.properties,
            children: []
          }

          //--Floor--
          let floorNode = {
            id: Config.FLOOR_TYPE + room.id.toString(),
            name: Config.FLOOR_TYPE +' '+ room.id.toString(),
            type: Config.FLOOR_TYPE,
            children: [],
            mesh: null
          }
          let floorInfo = this.ceilingAndfloor.floorMeshes.find(item => item.id == floorNode.id.toString());
          if (floorInfo)
            floorNode.mesh = floorInfo.mesh;

          //--Ceiling--
          let ceilingNode = {
            id: Config.CEILING_TYPE + room.id.toString(),
            name: Config.CEILING_TYPE +' '+ room.id.toString(),
            type: Config.CEILING_TYPE,
            children: [],
            mesh: null
          }
          let ceilingInfo = this.ceilingAndfloor.ceilingMeshes.find(item => item.id == ceilingNode.id.toString());
          if (ceilingInfo)
            ceilingNode.mesh = ceilingInfo.mesh;

          roomNode.children.push(floorNode);
          roomNode.children.push(ceilingNode);

          //--Wall--
          for (let i = 0; i < room.geometry.boundary.length; i++) {
            let wall = room.geometry.boundary[i];
            let wallNode = {
              id: wall.id,
              name: "Wall " + i,
              type: "Wall",
              children: [],
              mesh: null
            }
            let meshInfo = this.wallManager.wallMeshes.find(item => item.id == wall.id.toString());
            if (meshInfo)
              wallNode.mesh = meshInfo.mesh;

            for (let door of model.doors) {
              if (!door.geometry.positions.find(p => p.segment_id == wall.id))
                continue;

              let doorNode = {
                id: door.id,
                name: `[Door]${door.name}`,
                type: "Door",
                mesh: null
              }
              wallNode.children.push(doorNode);
            }
            for (let window of model.windows) {
              if (!window.geometry.positions.find(p => p.segment_id == wall.id))
                continue;

              let windowNode = {
                id: window.id,
                name: `[Window]${window.name}`,
                type: 'Window'
              }
              wallNode.children.push(windowNode);
            }
            roomNode.children.push(wallNode);
          }
          //end wall
          levelNode.children.push(roomNode);
        }
        hierarchy.push(levelNode);
      }

      

      resolve(hierarchy);
    });
  }


    //  //----Hierarchy------
    //  var element = json[i];
    //  var levelName = element.level || 'no_level';
    //  var h_levelNode = hierarchy.find(h => h.id === levelName);
    //  if (!h_levelNode) {
    //    h_levelNode = {
    //      id: levelName,
    //      name: levelName,
    //      type: "Level",
    //      children: [
    //        { id: levelName + '_rooms', name: 'Rooms', children: [] },
    //        { id: levelName + '_doors', name: 'Doors', children: [] }
    //      ]
    //    };
    //    hierarchy.push(h_levelNode);
    //  }
    //  //----End Hierarchy------
    //
    //  if (json[i].type === "Room") {
    //    //this.addRoom(json[i], 0);
    //    this.addCellingAndFloor(json[i], 0);
    //
    //    //Hierarchy
    //    h_levelNode.children.find(c => c.name === 'Rooms').children.push({
    //      id: element.id,
    //      name: element.name,
    //      type: 'Room',
    //    });
    //  }
    //  else if (json[i].type === "Door") {
    //    //Hierarchy
    //    h_levelNode.children.find(c => c.name === 'Doors').children.push({
    //      id: element.id,
    //      name: element.name
    //    });
    //  }
    //  //else if (json[i].type === "Wall") {
    //  //  this.addWall(json[i]);
    //  //}
    //}
    //this.nodes = hierarchy;
}
