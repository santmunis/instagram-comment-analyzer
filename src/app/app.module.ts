import { BrowserModule } from '@angular/platform-browser';
import {MatDialogModule, MAT_DIALOG_DEFAULT_OPTIONS} from '@angular/material/dialog';
import { NgModule } from '@angular/core';

import { SharedModule } from './shared/shared/shared.module';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { AppComponent } from './app.component';
import { PostsComponent } from './Instagram/posts/posts.component';
import { PostDialogComponent } from './Instagram/posts/post-dialog/post.dialog.component';


@NgModule({
  declarations: [
    AppComponent,
    PostsComponent,
    PostDialogComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    BrowserAnimationsModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  providers: [],
  entryComponents:[
    PostDialogComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
