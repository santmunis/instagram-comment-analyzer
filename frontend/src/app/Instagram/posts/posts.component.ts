import {Component, OnInit} from '@angular/core';
import {InstragamService} from 'src/app/shared/services/instragam.service';
import {Post} from './Models/posts.models';
import {Comment} from './Models/comment.models';
import {WatsonService} from 'src/app/shared/services/watson.service';
import {MatDialog} from '@angular/material/dialog';
import {PostDialogComponent} from './post-dialog/post.dialog.component';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent implements OnInit {

  public posts: Post[] = [];
  public comments: Comment[] = [];
  public loading: boolean = false;

  constructor(
    private _instagramService: InstragamService,
    private _watsonService: WatsonService,
    public dialog: MatDialog,
    private _snackBar: MatSnackBar
  ) {
  }

  ngOnInit(): void {
    const code = this.getParameterByName('code');
    if (code === null) {
      this.getAthorization();
    } else {
      this._instagramService.getToken(code).subscribe((response: any) => {
        this._instagramService.setToken(response.access_token);
        this.getPostUser();
      });
    }
  }
  public getParameterByName(name, url = window.location.href): string {
    name = name.replace(/[\[\]]/g, '\\$&');
    const regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)')
    const results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
  }

  public getAthorization(){
    this._instagramService.getAuthorization();
  }
  public getPostUser(): void {
    this._instagramService.getUserPerfilMidia()
      .subscribe(response => {
          this.posts = response.data;
        },
        error => this.loading = false);
  }

  public getCommentsPublication(post: Post): void {
    this.loading = true;
    this._instagramService.getCommentByPublicationUrl(post.permalink)
      .subscribe(response => {
        this.comments = response.data;
        const commentsText = this.comments.map(comment => comment.comment);
        this.getFellingtext(commentsText, post);
      }, error => {
        this.loading = false;
        this.notify('Está publicação não possui comentários', 'ok');
      });
  }

  public getFellingtext(texts: string[], post: Post): void {
    this._watsonService.getFellingText(texts).subscribe(response => {
      this.loading = false;
      this.dialog.open(PostDialogComponent, {
        width: '1200px',
        maxHeight: '90vh',
        data: {
          publication: post,
          fellingComments: response.data
        },
      });
    }, error => this.loading = false);
  }

  public notify(mensage: string, action: string): void {
    this._snackBar.open(mensage.toUpperCase(), action.toUpperCase(), {
      duration: 4000,
    });
  }
}
