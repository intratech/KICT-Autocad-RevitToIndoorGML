import { Scene, Vector3, Shape, Vector2, ShapeBufferGeometry, Mesh, Geometry, Line } from 'three';
import { HomeComponent } from '../home/home.component';
import { Room, JLevel, MeshInfo } from './models/DTO';
import { MaterialManager } from './material.manager';
import { ArrayUtil } from './utils/array.util';
import { Config } from '../config';

export class CeilingAndFloorManager{
  private scene: THREE.Scene;
  private minLevel: JLevel;
  private maxLevel: JLevel;
  private levels: JLevel[];
  private rooms: Room[];
  private scale: number;
  public ceilingMeshes: MeshInfo[] = [];
  public floorMeshes: MeshInfo[] = [];

    public init(homeComponent: HomeComponent): void{
        this.scene = homeComponent.scene;
    }

    public draw(rooms: Room[], levels: JLevel[], scale: number): void {
        this.levels = levels;
        this.rooms = rooms;
        this.scale = scale;
        this.minLevel = ArrayUtil.min(levels, (level: JLevel) => level.elevation);
        this.maxLevel = ArrayUtil.max(levels, (level: JLevel) => level.elevation);

        //let groupByLevel = this.groupBy(rooms, room => room.level_id);
        //for(let room of groupByLevel.get(311)){

        for(let room of this.rooms){
            this.drawFloorAndCeiling(room);
        }
    }
    private drawFloorAndCeiling(room: Room){
        if(room.id == 689595){
            console.log();
        }
        let floorVertices: Vector2[] = [];
        for(let i = 0; i < room.geometry.boundary.length; i++){
            let startPoint = room.geometry.boundary[i].points[0];
            let endPoint = room.geometry.boundary[i].points[1];

            if(i == 0){
                floorVertices.push(new Vector2(startPoint.x, startPoint.y));
                floorVertices.push(new Vector2(endPoint.x, endPoint.y));
            }
            else{
                floorVertices.push(new Vector2(endPoint.x, endPoint.y));
            }
        }
        let floorMaterial = MaterialManager.FLOOR_MATERIAL;
        if((room.is_elevator || room.is_escalator || room.is_stair) && !this.isLowest(room)){
            floorMaterial = MaterialManager.CLOSURE_SURFACE_MATERIAL;
        }

        let ceilingMaterial = MaterialManager.FLOOR_MATERIAL;
        if((room.is_elevator || room.is_escalator || room.is_stair) && !this.isHighest(room)){
            ceilingMaterial = MaterialManager.CLOSURE_SURFACE_MATERIAL;
        }

      if (floorVertices.length == 0) {
        return;
      }

        let shape = new Shape(floorVertices);
        let floorGeo = new ShapeBufferGeometry(shape);
        let floorMesh = new Mesh(floorGeo, floorMaterial);
        floorMesh.position.setZ(room.geometry.bouding_box.min.z);

        let ceilingMesh = new Mesh(floorGeo, ceilingMaterial);
        ceilingMesh.position.setZ(room.geometry.bouding_box.max.z);

      this.scene.add(floorMesh, ceilingMesh);

      //keep floor and ceiling meshes
      let floorInfo = new MeshInfo();
      floorInfo.id = Config.FLOOR_TYPE + room.id.toString();
      floorInfo.mesh = floorMesh;
      this.floorMeshes.push(floorInfo);

      let ceilingInfo = new MeshInfo();
      ceilingInfo.id = Config.CEILING_TYPE + room.id.toString();
      ceilingInfo.mesh = ceilingMesh;
      this.ceilingMeshes.push(ceilingInfo);

        // let lineGeo = new Geometry();
        // for(let point of floorVertices){
        //     lineGeo.vertices.push(new Vector3(point.x, point.y, 0));
        // }
        // let line = new Line(lineGeo, MaterialManager.LINE_MATERIAL)

        //this.scene.add(line);
    }
    
    private isLowest(room: Room){
        let anotherSameRoom: Room[] = [];

        if(room.is_elevator){
            anotherSameRoom = this.rooms.filter(r => r.is_elevator)
        }
        else if(room.is_escalator){
            anotherSameRoom = this.rooms.filter(r => r.is_escalator)
        }
        else if(room.is_stair){
            anotherSameRoom = this.rooms.filter(r => r.is_stair)
        }
        
        let roomLv = this.levels.find(lv => lv.id == room.level_id);

        let cX = (room.geometry.bouding_box.min.x + room.geometry.bouding_box.max.x)/2;
        let cY = (room.geometry.bouding_box.min.y + room.geometry.bouding_box.max.y)/2;
        
        
        for(let another of anotherSameRoom){
            let lv = this.levels.find(lv => lv.id == another.level_id);
            let _cX = (another.geometry.bouding_box.min.x + another.geometry.bouding_box.max.x)/2;
            let _cY = (another.geometry.bouding_box.min.y + another.geometry.bouding_box.max.y)/2;

            let deltaX = Math.abs(_cX - cX) * this.scale;
            let deltaY = Math.abs(_cY - cY) * this.scale;

            if(lv.elevation < roomLv.elevation && deltaX < 1000 && deltaY < 1000)
                return false;
        }
        
        return true;
    }
    private isHighest(room: Room){
        let anotherSameRoom: Room[] = [];

        if(room.is_elevator){
            anotherSameRoom = this.rooms.filter(r => r.is_elevator)
        }
        else if(room.is_escalator){
            anotherSameRoom = this.rooms.filter(r => r.is_escalator)
        }
        else if(room.is_stair){
            anotherSameRoom = this.rooms.filter(r => r.is_stair)
        }
        
        let roomLv = this.levels.find(lv => lv.id == room.level_id);
        let cX = (room.geometry.bouding_box.min.x + room.geometry.bouding_box.max.x)/2;
        let cY = (room.geometry.bouding_box.min.y + room.geometry.bouding_box.max.y)/2;
        
        for(let another of anotherSameRoom){
            let lv = this.levels.find(lv => lv.id == another.level_id);
            let _cX = (another.geometry.bouding_box.min.x + another.geometry.bouding_box.max.x)/2;
            let _cY = (another.geometry.bouding_box.min.y + another.geometry.bouding_box.max.y)/2;

            let deltaX = Math.abs(_cX - cX) * this.scale;
            let deltaY = Math.abs(_cY - cY) * this.scale;
            if(lv.elevation > roomLv.elevation  && deltaX < 1000 && deltaY < 1000)
                return false;
        }
        
        return true;
    }
}
