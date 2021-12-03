import { Vector3, Mesh } from "three";

export class Model {
  public model_name: string;
  public bounding_box: BoundingBox;
  public levels: JLevel[];
  public doors: Door[];
  public rooms: Room[];
  public windows: Window[];
  public scale: number;
}
export class JLevel
{
    public name: string;
    public id: number;
    public elevation: number;
}
export class BaseItem {
  public id: number;
  public type: string;
  public name: string;
  public level_id: number;
}

export class Room extends BaseItem {
  public is_elevator: boolean;
  public is_escalator: boolean;
  public is_stair: boolean;
  public geometry: RoomGeometry;
  public properties: Array<Prop>;
}

export class Door extends BaseItem {
  public geometry: DoorGeometry;
}

export class Window extends Door {
}

export class RoomGeometry {
  public bouding_box: BoundingBox;
  public position: Vector3;
  public boundary: Array<JLineSegment>;
}

export class BoundingBox {
  public min: Vector3;
  public max: Vector3;
}

export class JLineSegment {
  public id: number;
  public id2: number;
  public isClosureSurface: boolean;
  public points: Array<Vector3>;
}

export class Prop {
  public name: string;
  public value: string;
}

export class DoorGeometry {
  public positions: Array<Pos>;
  public width: number;
  public height: number;
}

export class Pos {
  public segment_id: number;
  public position: Vector3;
  public direction: Vector3;
}

export class DoorExtend {
  public id: number;
  public segment: JLineSegment;
  public room: Room;
  public door: Door;
  public position: THREE.Vector3;
  public dir: THREE.Vector3;
}

export class DoorPointList {
  public doorPoints: THREE.Vector3[];
  public id: number;
  public idWall: number;
  public item: DoorExtend;
  constructor() {
    this.doorPoints = [];
  }
}

export class MeshInfo {
  public id: string;
  public mesh: Mesh;
  constructor() {
    this.mesh = new Mesh();
  }
}
