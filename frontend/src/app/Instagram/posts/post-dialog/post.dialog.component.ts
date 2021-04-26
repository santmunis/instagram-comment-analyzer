import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';

@Component({
  selector: 'app-post.dialog',
  templateUrl: './post.dialog.component.html',
  styleUrls: ['./post.dialog.component.scss']
})
export class PostDialogComponent implements OnInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: {
      publication: any,
      fellingComments: any[]
    }
  ) {
  }

  ngOnInit(): void {

  }

}
