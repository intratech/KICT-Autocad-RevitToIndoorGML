import { Config } from "../config";
import { HomeComponent } from "../home/home.component";
import { BoundingBox } from "./models/DTO";
import * as THREE from 'three';
import { Mesh, Box3, Object3D } from "three";
import { TrackballControls } from 'three/examples/jsm/controls/TrackballControls';
import { OrbitControls } from "three/examples/jsm/controls/OrbitControls";

export class VisibilityManager {
  private scene: THREE.Scene;
    private camera: THREE.PerspectiveCamera;
    //private cameraOrthor: THREE.OrthographicCamera;
    private controls: TrackballControls

  public init(homeComponent: HomeComponent): void {
    this.scene = homeComponent.scene;
      this.camera = homeComponent.camera;
      //this.cameraOrthor = homeComponent.cameraOrthor;
    this.controls = homeComponent.controls;
  }
  public visibleNode(isVisible: boolean, node: any) {
    //console.log();
    if (!node)
      return;

    if (node.mesh && node.mesh.type && node.mesh.type == "Mesh")
      node.mesh.visible = isVisible;
    else if (node.children) {
      for (let child of node.children) {
        this.visibleNode(isVisible, child);
      }
    }
  }

  public showAllMeshes(isShow: boolean) {
    for (let i = 0; i < this.scene.children.length; i++) {
      let mesh = this.scene.children[i];
      if (mesh.name != Config.SKY_MESH_NAME) {
        if (mesh.type && mesh.type == "Mesh")
          mesh.visible = isShow;
      }
    }
  }

  public zoomToBox(box: BoundingBox) {
    try {
      if (box === null)
            return;

      //Perspective camera
      let center = new THREE.Vector3((box.max.x + box.min.x) / 2, (box.max.y + box.min.y) / 2, (box.max.z + box.min.z) / 2);
      let camDis = new THREE.Vector3().subVectors(box.max, box.min).length();
      let camDir = new THREE.Vector3().subVectors(center, this.camera.position);
      camDir.normalize();
      this.camera.position.x = center.x - camDir.x * camDis;
      this.camera.position.y = center.y - camDir.y * camDis;
      this.camera.position.z = center.z - camDir.z * camDis;

        //Orthor camera
        //this.cameraOrthor.pla
      this.controls.target = center;
    }
    catch (err) {
      return;
    }
  }

  public getBoundingBox(meshArr: Mesh[]) {
    //let box;
    let obj3D = new Object3D();
    for (let mesh of meshArr) {
      obj3D.add(mesh.clone());
      //mesh.geometry.computeBoundingBox();
      //let newBox = mesh.geometry.boundingBox;
      //if (box) {
      //  if (box.max.x < newBox.max.x) { box.max.x = newBox.max.x; }
      //  if (box.max.y < newBox.max.y) { box.max.y = newBox.max.y; }
      //  if (box.max.z < newBox.max.z) { box.max.z = newBox.max.z; }
      //  if (box.min.x > newBox.min.x) { box.min.x = newBox.min.x; }
      //  if (box.min.y > newBox.min.y) { box.min.y = newBox.min.y; }
      //  if (box.min.z > newBox.min.z) { box.min.z = newBox.min.z; }
      //}
      //else {
      //  box = new Box3().set(newBox.min, newBox.max);
      //}
    }
    //obj3D.visible = true;
    return new Box3().setFromObject(obj3D);
  }

  public getMeshesFromSelectedNode(node: any, meshesArr: Mesh[]) {
    if (!node)
      return;

    if (node.mesh)
      meshesArr.push(node.mesh);
    else if (node.children) {
      for (let child of node.children) {
        this.getMeshesFromSelectedNode(child, meshesArr);
      }
    }
  }


  //***********************************
  //Function content: On selected hierachy event
  //Developer: Nhan
  //Last modifier: Donny
  //Modification content: Add change color
  //Updated on: 28-Aug-19
  //***********************************
  //public onSelectNode(event) {
  //  var element = event.node.data;

  //  //start do something with selected mesh
  //  if (element.type === "Room") {
  //    //Fined mesh
  //    var mesh = this.scene.children.find(e => e.uuid === element.id) as THREE.Mesh;

  //    if (mesh) {

  //      //Change color
  //      this.changeColorSelectedNode(mesh);

  //      //Keep selected mesh
  //      this.INTERSECTED.uuid = element.id;
  //      this.INTERSECTED.mesh = mesh;

  //      //Zoom to selected mesh
  //      //mesh.geometry.computeBoundingBox();
  //      //var box = { "min": new THREE.Vector3(), "center": new THREE.Vector3(), "max": new THREE.Vector3() };
  //      //var boxMesh = mesh.geometry.boundingBox;
  //      //box.min.set(boxMesh.min.x, boxMesh.min.y, boxMesh.min.z);
  //      //box.max.set(boxMesh.max.x, boxMesh.max.y, boxMesh.max.z);
  //      //box.center.set((box.min.x + box.max.x) / 2, (box.min.y + box.max.y) / 2, (box.min.z + box.max.z) / 2);
  //      //this.ZoomToBox(box);

  //      //properties
  //      if (mesh.userData) {
  //        this.selectedItemProperties = mesh['userData'] as [];
  //      }
  //      else {
  //        this.selectedItemProperties = [];
  //      }
  //    }

  //  }
  //}

  //***********************************
  //Function content: Change color with selected node
  //Developer: Donny
  //Last modifier: 
  //Modification content:
  //Updated on: 28-Aug-19
  //***********************************
  //private _lastOriginalColor;
  //private _lastMesh;
  //private changeColorSelectedNode(mesh): void {
  //  console.log(mesh);

  //  //return original for last selectd mesh
  //  if (this._lastOriginalColor && this._lastMesh) {
  //    this._lastMesh.material.color = this._lastOriginalColor;
  //  }

  //  //Set new color for selected mesh
  //  this._lastOriginalColor = mesh.material.color;
  //  this._lastMesh = mesh;
  //  mesh.material.color = new THREE.Color(0xff0000);
  //}

  


  //  //this.raycaster.setFromCamera(this.mouse, this.camera);
  //  //var intersects = this.raycaster.intersectObjects(this.scene.children);
  //  //if (intersects.length > 0) {
  //  //  if (this.INTERSECTED != intersects[0].object) {
  //  //    if (this.INTERSECTED) this.INTERSECTED.material.emissive.setHex(this.INTERSECTED.currentHex);
  //  //    this.INTERSECTED = intersects[0].object;
  //  //    this.INTERSECTED.currentHex = this.INTERSECTED.material.emissive.getHex();
  //  //    this.INTERSECTED.material.emissive.setHex(0xff0000);
  //  //  }
  //  //} else {
  //  //  if (this.INTERSECTED) this.INTERSECTED.material.emissive.setHex(this.INTERSECTED.currentHex);
  //  //  this.INTERSECTED = null;
  //  //}
  //}

  //***********************************
  //Function content: Zoom to bounding box
  //Developer: Donny
  //Last modifier:
  //Modification content: 
  //Updated on: 28-Aug-19
  //***********************************
  //private _zoomRatio = 2;
  //private _zAxisMinus = new THREE.Vector3(0, 0, -1);
  //private _yUpVec = new THREE.Vector3(0, 1, 0);
  //private ZoomToBox(Bbox) {
  //  try {
  //    if (!Bbox) return;
  //    //var camDir = new THREE.Vector3();
  //    //this.camera.getWorldDirection(camDir);
  //    //var upDir = new THREE.Vector3(this.camera.up.x, this.camera.up.y, this.camera.up.z);

  //    //var maxx = Bbox.max.x - Bbox.min.x;
  //    //var maxy = Bbox.max.y - Bbox.min.y;
  //    //var maxz = Bbox.max.z - Bbox.min.z;
  //    //var max = Math.max(maxx, maxy);
  //    //max = Math.max(max, maxz);
  //    //
  //    //var distance = max * this._zoomRatio;
  //    //var newCamPos = new THREE.Vector3();
  //    //newCamPos.x = Bbox.center.x - this._zAxisMinus.x * distance;
  //    //newCamPos.y = Bbox.center.y - this._zAxisMinus.y * distance;
  //    //newCamPos.z = Bbox.center.z - this._zAxisMinus.z * distance;

  //    this.camera.position.set(Bbox.center.x, Bbox.center.y, 30);
  //    this.camera.lookAt(Bbox.center);
  //    this.camera.updateProjectionMatrix();
  //    this.camera.up.set(this._yUpVec.x, this._yUpVec.y, this._yUpVec.z);
  //    this.camera.updateProjectionMatrix();
  //  }
  //  catch (err) {
  //    return;
  //  }
  //}

  //private addDoor(startpos, endpos, doorPosition) {
  //  console.log('add_door...');
  //  if (startpos && endpos && doorPosition) {
  //    var width = 3.4;
  //    var heigh = 6;
  //    var pos1 = new THREE.Vector3(doorPosition.x, doorPosition.y, doorPosition.z);
  //    var pos2 = new THREE.Vector3(doorPosition.x, doorPosition.y, doorPosition.z);

  //    var dir = new THREE.Vector3(Math.round(endpos.x - startpos.x), Math.round(endpos.y - startpos.y), Math.round(endpos.z - startpos.z));
  //    dir.normalize();
  //    var u = new THREE.Vector3(dir.x, dir.y, dir.z).multiplyScalar(width / 2);
  //    var v = new THREE.Vector3(dir.x, dir.y, dir.z).multiplyScalar(-width / 2);
  //    pos1.add(u);
  //    pos2.add(v);

  //    var pos3 = new THREE.Vector3(pos1.x, pos1.y, pos1.z + heigh);
  //    var pos4 = new THREE.Vector3(pos2.x, pos2.y, pos2.z + heigh);
  //    this.geometry = new THREE.Geometry();
  //    this.geometry.vertices.push(pos1, pos3, pos4, pos2);
  //    var line = new THREE.Line(this.geometry, this.material2);
  //    line.name = "Door";
  //    this.scene.add(line);
  //  }
  //  else {
  //    console.log("Door has no direction.");
  //  }
  //}
  //private addWall(wall: any): void {
  //  var wallGeo = new THREE.Geometry();

  //  for (var i = 0; i < wall.geometry.faces.length; i++) {
  //    var f = wall.geometry.faces[i];
  //    for (let j = 0; j < f.length; j++) {
  //      //let startPoint = f[i];
  //      //let endPoint = f[i + 1];
  //      wallGeo.vertices.push(new THREE.Vector3(f[j].x, f[j].y, f[j].z));
  //    }
  //  }

  //  var color = new THREE.Color(0.2, 0.2, 0.2);
  //  var mat = new THREE.MeshPhongMaterial({ color: color, side: THREE.DoubleSide });
  //  var mesh = new THREE.Mesh(wallGeo, mat);
  //  this.scene.add(mesh);
  //}

  ////***********************************
  ////Function content: Draw floor and celling surface
  ////Developer: Donny
  ////Last modifier:
  ////Modification content:
  ////Updated on: 28-Aug-19
  ////***********************************
  //private addCellingAndFloor(room, separateIndex: number) {

  //  //Create new Shape
  //  var floorShape = new THREE.Shape();

  //  //Start point
  //  var startPoint = room.geometry.boundary[0];
  //  floorShape.moveTo(startPoint.x, startPoint.y);

  //  //Draw shape boudary
  //  for (let i = 0; i < room.geometry.boundary.length; i += 2) {
  //    var endPoint = room.geometry.boundary[i + 1];
  //    floorShape.lineTo(endPoint.x, endPoint.y);
  //  }

  //  //Draw floor surfaces
  //  var opacity = 0.8;
  //  var color = new THREE.Color(0.5, 0.5, 0.5);
  //  var geometry = new THREE.ShapeGeometry(floorShape);
  //  var material = new THREE.MeshBasicMaterial({ color: color, side: THREE.DoubleSide, opacity: opacity, transparent: true });
  //  var mesh = new THREE.Mesh(geometry, material);
  //  mesh.position.setZ(startPoint.z);
  //  this.scene.add(mesh);

  //  //Draw celling surfaces
  //  geometry = new THREE.ShapeGeometry(floorShape);
  //  mesh = new THREE.Mesh(geometry, material);
  //  if (room.geometry.bouding_box.min.z === 0)
  //    mesh.position.setZ(room.geometry.bouding_box.max.z - separateIndex);
  //  else mesh.position.setZ(room.geometry.bouding_box.max.z - (room.geometry.bouding_box.max.z - room.geometry.bouding_box.min.z) / 2);
  //  this.scene.add(mesh);
  //}

  ////***********************************
  ////Function content: Create Rooms have doors inside
  ////Developer: Donny
  ////Last modifier:
  ////Modification content:
  ////Updated on: 27-Aug-19
  ////***********************************
  //private _doorRoom: any = {};
  //private CreateInteriorRoom(Doors, Rooms) {
  //  if (Rooms && Doors) {
  //    var doorLine = [];

  //    //Loop Doors
  //    for (let i = 0; i < Doors.length; i++) {
  //      var lines = this.getNearestLineOfRoom(Doors[i], Rooms);

  //      //Draw door1 on the wall
  //      if (lines["line1"]) {
  //        var newDoorPosition1 = this.GetPerpendiculaire(lines["line1"].startPoint, lines["line1"].endPoint, lines["line1"].door.geometry.position);
  //        this.addDoor(lines["line1"].startPoint, lines["line1"].endPoint, newDoorPosition1);

  //        //Get Door id attach in the room
  //        this._doorRoom[Doors[i].id.toString()] = lines;
  //      }

  //      //Draw door2 on the wall
  //      if (lines["line2"]) {
  //        var newDoorPosition2 = this.GetPerpendiculaire(lines["line2"].startPoint, lines["line2"].endPoint, lines["line2"].door.geometry.position);
  //        this.addDoor(lines["line2"].startPoint, lines["line2"].endPoint, newDoorPosition2);
  //      }
  //    }
  //  }
  //}

  ////***********************************
  ////Function content: Create surface from 2D -> 1 point of door and all surface
  ////Developer: Donny
  ////Last modifier:
  ////Modification content:
  ////Updated on: 27-Aug-19
  ////***********************************
  //private getNearestLineOfRoom(door, rooms) {
  //  var min1: number = null;
  //  var min2: number = null;
  //  var line1: baseLine;
  //  var line2: baseLine;
  //  var indexRoom: number = 0;
  //  var indexBoundary: number = 0;
  //  var tole = 0.001//radian

  //  //Loop all Rooms
  //  if (door.geometry.direction) {
  //    for (let i = 0; i < rooms.length; i++) {

  //      //For loop all lines of room
  //      for (let j = 0; j < rooms[i].geometry.boundary.length; j += 2) {
  //        var startPoint1 = rooms[i].geometry.boundary[j];
  //        var endPoint1 = rooms[i].geometry.boundary[j + 1];
  //        var dir1 = new THREE.Vector3();
  //        dir1.subVectors(endPoint1, startPoint1);
  //        dir1.normalize();
  //        var doorDir1 = new THREE.Vector3(door.geometry.direction.x, door.geometry.direction.y, door.geometry.direction.z);
  //        var angle1 = dir1.dot(doorDir1);

  //        if (angle1 > -tole && angle1 < tole) {

  //          //Need to check door is middle of surface line


  //          //Check 2 nearest distance from doorPoint to all surfaces of room
  //          var d = this.distancePointToLine(door.geometry.position, startPoint1, endPoint1);
  //          console.log("Door id: " + door.id + ", distance: " + d);
  //          if (!min1) {//Get nearest point1
  //            min1 = d;
  //            indexRoom = i;
  //            indexBoundary = j;

  //            //Get nearest line
  //            line1 = { startPoint: startPoint1, endPoint: endPoint1, distance: min1, door, room: rooms[i] };
  //          }
  //          else if (min1 > d) {
  //            min1 = d;
  //            indexRoom = i;
  //            indexBoundary = j;

  //            //Get nearest line
  //            line1 = { startPoint: startPoint1, endPoint: endPoint1, distance: min1, door, room: rooms[i] };
  //          }
  //        }
  //      }
  //      console.log('');
  //    }

  //    //Loop all Rooms
  //    for (let i = 0; i < rooms.length; i++) {
  //      for (let j = 0; j < rooms[i].geometry.boundary.length; j += 2) {
  //        if (j === indexBoundary && i === indexRoom)
  //          continue;

  //        var startPoint2 = rooms[i].geometry.boundary[j];
  //        var endPoint2 = rooms[i].geometry.boundary[j + 1];
  //        var dir2 = new THREE.Vector3();
  //        dir2.subVectors(endPoint2, startPoint2);
  //        dir1.normalize();
  //        var doorDir2 = new THREE.Vector3(door.geometry.direction.x, door.geometry.direction.y, door.geometry.direction.z);
  //        var angle2 = dir2.dot(doorDir2);

  //        if (angle2 > -tole && angle2 < tole) {
  //          //Check 2 nearest distance from doorPoint to all surfaces of room
  //          var d = this.distancePointToLine(door.geometry.position, startPoint2, endPoint2);
  //          if (!min2) {//Get nearest point1
  //            min2 = d;

  //            //Get nearest line
  //            line2 = { startPoint: startPoint2, endPoint: endPoint2, distance: min2, door, room: rooms[i] };
  //          }
  //          else if (min2 > d) {
  //            min2 = d;

  //            //Get nearest line
  //            line2 = { startPoint: startPoint2, endPoint: endPoint2, distance: min2, door, room: rooms[i] };
  //          }
  //        }
  //      }
  //    }
  //  }

  //  //Create new obj to return 2 lines of door
  //  var surfacesAndDoor = { "line1": null, "line2": null };
  //  if (line1 && line2 && line2.distance < line1.distance * 2) {
  //    surfacesAndDoor["line1"] = line1;
  //    surfacesAndDoor["line2"] = line2;
  //  }
  //  else if (line1) {
  //    surfacesAndDoor["line1"] = line1;
  //    surfacesAndDoor["line2"] = null;
  //  }

  //  //Get new doorPoint
  //  return surfacesAndDoor;
  //}

  ////***********************************
  ////Function content: Calculate distance from 1 point line(Distance from A to MH)
  ////Developer: Donny
  ////Last modifier:
  ////Modification content:
  ////Updated on: 27-Aug-19
  ////***********************************
  //private distancePointToLine(A: THREE.Vector3, M: THREE.Vector3, H: THREE.Vector3): number {
  //  if (A && M && H) {
  //    var MH = new THREE.Vector3(H.x - M.x, H.y - M.y, H.z - M.z);

  //    //Calculate coordinate of H_ (perpendiculaire:hinh chieu vuong goc)
  //    var H_ = this.GetPerpendiculaire(M, H, A);

  //    //Calculate distance
  //    var AH_ = new THREE.Vector3(A.x - H_.x, A.y - H_.y, A.z - H_.z);
  //    var d = AH_.length();
  //    return d;
  //  }
  //}

  ////***********************************
  ////Function content: Tim hinh chieu H cua A len P1P2
  ////Developer: Donny
  ////Last modifier:
  ////Modification content:
  ////Updated on: 27-Aug-19
  ////***********************************
  //private GetPerpendiculaire(P1, P2, A): THREE.Vector3 {
  //  try {
  //    //Tim hinh chieu H cua A len P1P2
  //    var H = new THREE.Vector3();
  //    var P1P2 = new THREE.Vector3(P2.x - P1.x, P2.y - P1.y, P2.z - P1.z);
  //    H.x = P1P2.x * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.x;
  //    H.y = P1P2.y * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.y;
  //    H.z = P1P2.z * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.z;
  //    return H;
  //  }
  //  catch (err) { }
  //}

  //public hide() {
  //  if (this.INTERSECTED) {
  //    this.INTERSECTED.mesh.visible = !this.INTERSECTED.mesh.visible;
  //  }
  //}
  //public unhideAll() {
  //  this.scene.children.forEach(element => {
  //    if (element.constructor.name !== "Mesh")
  //      return;

  //    element.visible = true;
  //  });
  //}
  //public hideOther() {
  //  var relatedID;

  //  this.scene.children.forEach(element => {
  //    if (element.constructor.name !== "Mesh")
  //      return;

  //    if (element.uuid !== this.INTERSECTED.uuid) {
  //      element.visible = false;
  //    }
  //  });

  //  var kindOfId;
  //  if (this.INTERSECTED.mesh.name.indexOf("Room") > -1)
  //    kindOfId = "Room";
  //  else kindOfId = "Door";
  //  relatedID = this.getRelatedID(this.INTERSECTED.uuid.toString(), this._doorRoom, kindOfId);

  //  this.scene.children.forEach(element => {
  //    console.log(element);
  //    for (var i = 0; i < relatedID.length; i++) {
  //      if (element.uuid.toString() === relatedID[i]) {
  //        element.visible = true;
  //      }
  //    }
  //  });
  //}

  ////***********************************
  ////Function content: find id from door or room
  ////Developer: Donny
  ////Last modifier:
  ////Modification content: 
  ////Updated on: 28-Aug-19
  ////***********************************
  //private getRelatedID(id, IDList, kindOfid) {
  //  if (!id || !IDList) return;

  //  //Create new arr
  //  var relatedIdArr = [];

  //  var ID = id.toString();
  //  relatedIdArr.push(ID);

  //  //Start search if Door
  //  if (kindOfid === "Door") {
  //    if (IDList[ID]) {
  //      relatedIdArr.push(IDList[ID]["line1"].room.id.toString());
  //      if (IDList[ID]["line2"]) {
  //        relatedIdArr.push(IDList[ID]["line2"].room.id.toString());
  //      }
  //    }
  //  }

  //  //Start search if Room
  //  if (kindOfid === "Room") {
  //    for (let key in IDList) {
  //      if (IDList[key]["line1"].room.id.toString() === ID) {
  //        relatedIdArr.push(key);
  //      }
  //      else if (IDList[key]["line2"] && IDList[key]["line2"].room.id.toString() === ID) {
  //        relatedIdArr.push(key);
  //      }
  //    }
  //  }

  //  return relatedIdArr;
  //}
}
