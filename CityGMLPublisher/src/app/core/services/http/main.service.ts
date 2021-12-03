import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MainService {
  constructor(private http: HttpClient) { }

  getObjects(url: string) {
    return this.http.get(url);
  }
}
