import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InstragamService {

  private readonly _appId: number = environment.appId;
  private readonly _apiUrl: string = environment.apiUrl;
  private readonly _urlSiteRedirect: string = environment.urlSiteRedirect;
  private readonly _appToken: string = environment.superSecretAppToken;
  private readonly _appApiUrl: string = environment.superSecretAppToken;
  private readonly token: string ="IGQVJXb2xBb3NWVERJR2VtWXdrWi1LcGtSZADJFMU1kMlJoazJ6ZAlFqT2VJNHo3RjlTaTJ2a3RfdFNEdzVvWnRHQmpwUFlXcnF1ZA3pnQnNsalU3SHY3d0JlXzA0bWRZAbFhpWHRPbzkzdFk1MGQzUnZAWbAZDZD";
  constructor(
    private _http: HttpClient,
  ) { }


  public getInstagramAuthorizhation(): Observable<any> {
    return this._http.get(`https://api.instagram.com/oauth/authorize?client_id=${this._appId}
    &redirect_uri=${this._urlSiteRedirect}&scope=user_profile,user_medi&response_type=code`);
  }


  public getUserPerfilInformation(): Observable<any> {
    return this._http.get(`https://graph.instagram.com/me?fields=id,username,media&access_token=${this.token}`);

  }

  public getUserPerfilMidia(): Observable<any> {
    return this._http.get(`https://graph.instagram.com/me/media?fields=id,caption,media_url,permalink&access_token=${this.token}`);
  }

  public getCommentByPublicationUrl(urlPublication: string): Observable<any> {
    return this._http.post(`${this._apiUrl}/instagram/publication/comments`, {
      urlPublication
    });
  }


}
