import { HomeComponent } from '../home/home.component';
import { Room, Door, Window, JLineSegment, BoundingBox, DoorExtend, MeshInfo } from './models/DTO';
import * as THREE from 'three';
import { MathUtil } from './utils/math.util';
import { Transform } from 'stream';
import { MaterialManager } from './material.manager';
import { Vector2, ShapeBufferGeometry, Mesh, Line } from 'three';

export class WallManager {

    constructor(){

  }

  doors: Door[];
  windows: Window[];
  wallMeshes: MeshInfo[] = [] ;

    private scene: THREE.Scene;

    public init(homeComponent: HomeComponent): void{
        this.scene = homeComponent.scene;
    }

  public addRoom(rooms: Room[], doors: Door[], windows: Window[]) {
    this.doors = doors;
    this.windows = windows;
    for (var i = 0; i < rooms.length; i++) {
      //if (rooms[i].id == 704358) {
      var doormap = MathUtil.FindDoorsWindowsInRoom(rooms[i], doors, windows);
      this.createRoom(rooms[i], doormap);
      //}
    }
  }

  public createRoom(room: Room, doormap: Map<number, DoorExtend>) {
      var boundary = room.geometry.boundary;
    for (var i = 0; i < boundary.length; i++) {
      //if (boundary[i].id == 815) {
      this.createSurface(boundary[i],  room.geometry.bouding_box, doormap);
      //}
    }
  }

  public createSurface(seg: JLineSegment, box: BoundingBox, doormap: Map<number, DoorExtend>) {
    let startpoint = new THREE.Vector3(seg.points[0].x, seg.points[0].y, seg.points[0].z);
    let endpoint = new THREE.Vector3(seg.points[1].x, seg.points[1].y, seg.points[1].z);
    var v = new THREE.Vector3();
    v.subVectors(startpoint, endpoint);
    v.normalize();
    var a = new THREE.Vector3(0, 0, 1);
    var n = a.cross(v);
    n.normalize();
    var height = box.max.z - box.min.z;

    //Create 2D shape information
    var point1 = new THREE.Vector2(startpoint.x, startpoint.y);
    var point2 = new THREE.Vector2(startpoint.x + n.x * height, startpoint.y + n.y * height);
    var point3 = new THREE.Vector2(endpoint.x + n.x * height, endpoint.y + n.y * height);
    var point4 = new THREE.Vector2(endpoint.x, endpoint.y);


    var shape = new THREE.Shape([point1, point2, point3, point4, point1]);

    //Make the hole of door,window
    var doorList = MathUtil.extendDoorByPoint(doormap, seg, n);
    if (doorList && doorList.size != 0) {
      var count = 0;
      doorList.forEach(door => {
        //if (count == 0) {
          var path = new THREE.Path(door.doorPoints.map(p => new Vector2(p.x, p.y)));
          shape.holes.push(path);
        //}
        count++;
      });
    }

    //Test line of door
    //if (doorList && doorList.size != 0) {
    //  doorList.forEach(door => {
    //    var geoline = new THREE.Shape(door.doorPoints.map(p => new Vector2(p.x, p.y)));
    //    this.scene.add(new Line(new ShapeBufferGeometry(geoline), MaterialManager.LINE_MATERIAL));
    //  });
    //}
       

    var geometry = new THREE.ShapeBufferGeometry(shape);

    var material = MaterialManager.FLOOR_MATERIAL;
    if (seg.isClosureSurface == true)
      material = MaterialManager.CLOSURE_SURFACE_MATERIAL;
    

    geometry.translate(-startpoint.x, -startpoint.y, 0);

    var mesh = new THREE.Mesh(geometry, material);
    mesh.rotateOnWorldAxis(v, Math.PI / 2);

    mesh.position.set(startpoint.x, startpoint.y, box.min.z);

    this.scene.add(mesh);

    var wallInfo = new MeshInfo();
    wallInfo.id = seg.id.toString();
    wallInfo.mesh = mesh;
    this.wallMeshes.push(wallInfo);
  }

  groupBy(list, keyGetter) {
    const map = new Map();
    list.forEach((item) => {
      const key = keyGetter(item);
      const collection = map.get(key);
      if (!collection) {
        map.set(key, [item]);
      } else {
        collection.push(item);
      }
    });
    return map;
  }
}
