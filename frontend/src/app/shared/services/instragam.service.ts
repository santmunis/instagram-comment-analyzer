import {Injectable} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InstragamService {

  private readonly _appId: string = environment.appId;
  private readonly _apiUrl: string = environment.apiUrl;
  private readonly _urlSiteRedirect: string = environment.urlSiteRedirect;
  private readonly _appToken: string = environment.superSecretAppToken;
  private readonly _appApiUrl: string = environment.superSecretAppToken;
  private token: string = "IGQVJYck93RkptT1NjakhMdFJSNGxsQnNNTXpjQXNJR0lGWjI4a3J3MEw2SHZAPTUNpYUxoTjdncUNUcG83TDFkLTZAoYzYyLTNYeVpER0gySHN4dUVhZAktEV3pueF9CVzE2cG80dVdzUHZAZAT2xDUzE4WAZDZD";

  constructor(
    private _http: HttpClient,
  ) {
  }

  public getAuthorization() {
    window.open(`https://api.instagram.com/oauth/authorize?client_id=${this._appId}&redirect_uri=https://localhost:4200/&scope=user_profile,user_media&response_type=code`);
  }

  public getToken(code_user) {

    const formData = new FormData();
    formData.append('client_id', this._appId);
    formData.append('client_secret', this._appToken);
    formData.append('grant_type', 'authorization_code');
    formData.append('redirect_uri', 'https://localhost:4200/');
    formData.append('code', code_user);
    return  this._http.post('https://api.instagram.com/oauth/access_token' , formData);

  }

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

  public setToken(token: string) {
    this.token = token;
  }

}
