import { Room, Door, Window, DoorExtend, DoorPointList, JLineSegment } from "../models/DTO";
import * as THREE from 'three';
import { Vector3, Mesh, Box3, Object3D } from "three";

export class MathUtil {
  public static _widthDefault: number = 0.8;
  public static _heighDefault: number = 2.1;
  public static _betweenThickness: number = 1;
  public static FindDoorsWindowsInRoom(room: Room, doors: Door[], windows: Window[]): Map<number, DoorExtend>
  {
    if (room != null && doors != null) {
    //Create list of door inside Room
    var roomDoorWindowList = new Map<number, DoorExtend>();

    //Check id of room is exist in door and get door's list
    var segmentsRoom = room.geometry.boundary;

    //Loop in each segment of room
    for (var i = 0; i < segmentsRoom.length; i++) {
      //Loop all doors
      for (let door of doors)
      {
        var doorGeo = door.geometry;

        if (doorGeo.width > 0)
          this._widthDefault = doorGeo.width;
        if (doorGeo.height > 0)
          this._heighDefault = doorGeo.height;

        //Loop in door position of each segment
        for(var doorPos of door.geometry.positions)
        {
          if (doorPos.segment_id == segmentsRoom[i].id) {
            var newDoor = new DoorExtend();
            newDoor.door = door;
            newDoor.room = room;
            newDoor.id = door.id;
            newDoor.segment = segmentsRoom[i];
            newDoor.position = doorPos.position;
            newDoor.dir = doorPos.direction;
            roomDoorWindowList.set(door.id, newDoor);
          }
        }
      }

      //Loop all windows
      for(let window of windows)
      {
        var windowGeo = window.geometry;

        if (windowGeo.width > 0)
          this._widthDefault = windowGeo.width;
        if (windowGeo.height > 0)
          this._heighDefault = windowGeo.height;

        //Loop in door position of each segment
        for(let doorPos of window.geometry.positions)
        {
          if (doorPos.segment_id == segmentsRoom[i].id) {
            var newDoor = new DoorExtend();
            newDoor.door = window;
            newDoor.room = room;
            newDoor.id = window.id;
            newDoor.segment = segmentsRoom[i];
            newDoor.position = doorPos.position;
            newDoor.dir = doorPos.direction;
            roomDoorWindowList.set(window.id,newDoor);
          }
        }
      }
    }
    return roomDoorWindowList;
  }
  return null;
  }

  public static extendDoorByPoint(doorsList: Map<number, DoorExtend>, seg: JLineSegment, n: THREE.Vector3) {
    if (doorsList != null) {
      var doorPointList = new Map<number, DoorPointList>();

      //Adjust width of doors, windows
      let doorListOnWall = new Array<DoorExtend>();
      doorsList.forEach(item => {
        if (seg.id == item.segment.id) {
          doorListOnWall.push(item);
        }
      });
      this.adjustDoorWidth(doorListOnWall, this._betweenThickness);

      //Start get door points
      doorsList.forEach(item => {
        if (seg.id == item.segment.id) {
          this.GetDoorPoints(item, this._widthDefault, this._heighDefault, doorPointList, n);
        }
      });

      return doorPointList;
    }
    return null;
  }

  public static GetDoorPoints(doorExtend: DoorExtend, widthDefault: number, heighDefault: number, doorList: Map<number, DoorPointList>, n: THREE.Vector3) {

    if (doorExtend.door.geometry.width > 0)
      widthDefault = doorExtend.door.geometry.width;
    if (doorExtend.door.geometry.height > 0)
      heighDefault = doorExtend.door.geometry.height;

    let startPoint = new THREE.Vector3(doorExtend.segment.points[0].x, doorExtend.segment.points[0].y, doorExtend.segment.points[0].z);
    let endPoint = new THREE.Vector3(doorExtend.segment.points[1].x, doorExtend.segment.points[1].y, doorExtend.segment.points[1].z);
    let doorPos = this.getProjectionLine(startPoint, endPoint, doorExtend.position);

    //specification of door,window for width and height -> must be inside wall of room
    let widthSpec1 = new THREE.Vector3().subVectors(startPoint, doorPos).length() * 2;
    let widthSpec2 = new THREE.Vector3().subVectors(endPoint, doorPos).length() * 2;
    let heightSpec = doorExtend.room.geometry.bouding_box.max.z - doorExtend.room.geometry.bouding_box.min.z;

    //set thickness of door with floor
    let thickness = 0.5;
    var thicknessVec = new THREE.Vector3(n.x, n.y, n.z);
    thicknessVec.multiplyScalar(thickness);
    doorPos.add(thicknessVec);

    //set height of window with floor
    let h = doorExtend.position.z - doorExtend.room.geometry.bouding_box.min.z;
    let hVec = new THREE.Vector3(n.x, n.y, n.z);
    hVec.multiplyScalar(h);

    //Get P1
    var point1 = new Vector3();
    point1.subVectors(startPoint, endPoint).normalize().multiplyScalar(widthDefault / 2);
    if (point1.length() >= (widthSpec1 / 2 - thickness)) {
      point1 = new Vector3();
      point1.subVectors(startPoint, endPoint).normalize().multiplyScalar(widthSpec1 / 2 - thickness).add(doorPos);
    }
    else point1.add(doorPos);

    //Get P4
    var point4 = new Vector3();
    point4.subVectors(endPoint, startPoint).normalize().multiplyScalar(widthDefault / 2);
    if (point4.length() >= (widthSpec2 / 2 - thickness)) {
      point4 = new Vector3();
      point4.subVectors(endPoint, startPoint).normalize().multiplyScalar(widthSpec2 / 2 - thickness).add(doorPos);
    }
    else point4.add(doorPos);

    //Set height for window
    let newheightSpec = heightSpec - thickness;
    if (doorExtend.door.type == "Window") {
      point1.add(hVec);
      point4.add(hVec);
      newheightSpec = newheightSpec - hVec.length();
    }

    //Get P2
    var point2 = new Vector3();
    point2.addVectors(n.clone().multiplyScalar(heighDefault), point1);

    //Get P3
    var point3 = new Vector3();
    point3.addVectors(n.clone().multiplyScalar(heighDefault), point4);

    //Check P2,P3 out of room
    var P4P3 = new THREE.Vector3();
    P4P3.subVectors(point3, point4);
    
    if (P4P3.length() >= newheightSpec) {
      let move = Math.abs(P4P3.length() - newheightSpec);
      point2.sub(n.clone().multiplyScalar(move).add(thicknessVec));
      point3.sub(n.clone().multiplyScalar(move).add(thicknessVec));
    }

    var doorPoints = new DoorPointList();

    doorPoints.doorPoints.push(point1);
    doorPoints.doorPoints.push(point2);
    doorPoints.doorPoints.push(point3);
    doorPoints.doorPoints.push(point4);
    doorPoints.doorPoints.push(point1);
    doorPoints.id = doorExtend.id;
    doorPoints.idWall = doorExtend.segment.id;
    doorPoints.item = doorExtend;

    doorList.set(doorExtend.id, doorPoints);
  }

  public static getProjectionLine(P1: THREE.Vector3, P2: THREE.Vector3, A: THREE.Vector3) {
    var P1P2 = new THREE.Vector3().subVectors(P2, P1);
    var H = new THREE.Vector3();
    H.x = P1P2.x * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.x;
    H.y = P1P2.y * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.y;
    H.z = P1P2.z * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.z;
    return H;
  }


  public static adjustDoorWidth(doorExtend: DoorExtend[], betweenThickness: number) {

    for (let i = 0; i < doorExtend.length; i++) {
      let doorExtend_i = doorExtend[i];
      let Doorpos1 = doorExtend[i].position;
      if (doorExtend[i].door.geometry.width < 0)
        doorExtend[i].door.geometry.width = this._widthDefault;
      let Width1 = doorExtend[i].door.geometry.width;
      doorExtend.splice(i, 1);

      for (let j = 0; j < doorExtend.length; j++) {
        let DoorPos2 = doorExtend[j].position;
        if (doorExtend[j].door.geometry.width < 0)
          doorExtend[j].door.geometry.width = this._widthDefault;
        let Width2 = doorExtend[j].door.geometry.width;

        //Compare distance and width
        let Dis = new THREE.Vector3().subVectors(DoorPos2, Doorpos1).length();
        if (Dis <= (Width1 + Width2)/2) {
          doorExtend_i.door.geometry.width = Dis - betweenThickness;
          doorExtend[j].door.geometry.width = Dis - betweenThickness;
        }
      }
    }
  }

  //public static getBoundingBox(mesh: Object3D) {
  //  if (!mesh || !mesh.geometry)
  //    return;
  //  mesh.geometry.computeBoundingBox();
  //  let box = mesh.geometry.boundingBox;
  //  return box;
  //}

  public static getBoxByObjArr(meshArr: Array<Object3D>) {
    var extendBox: Box3 = new Box3();
    if (!meshArr)
      return extendBox;

    for (let i = 0; i < meshArr.length; i++) {
      //let box = this.getBoundingBox(meshArr[i]);
      let box = new Box3().setFromObject(meshArr[i]);
      this.extendBoxbyBox(extendBox, box);
    }
    return extendBox;
  }

  public static extendBoxbyBox(extendBox: Box3, box: Box3) {
    if (!box)
      return;
    if (!extendBox) {
      extendBox = box;
      return;
    }
      
    if (extendBox.min.x > box.min.x)
      extendBox.min.x = box.min.x;
    if (extendBox.min.y > box.min.y)
      extendBox.min.y = box.min.y;
    if (extendBox.min.z > box.min.z)
      extendBox.min.z = box.min.z;
    if (extendBox.max.x < box.max.x)
      extendBox.max.x = box.max.x;
    if (extendBox.max.y < box.max.y)
      extendBox.max.y = box.max.y;
    if (extendBox.max.z < box.max.z)
      extendBox.max.z = box.max.z;
  }
}


