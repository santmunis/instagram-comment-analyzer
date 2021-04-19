import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class WatsonService {

  constructor(private _http: HttpClient) { }

  public getFellingText(texts: string[]): Observable<any> {
    return this._http.post(`${environment.apiUrl}/Watson/analyze/feeling/text`, {
      texts
    });
  }
}
