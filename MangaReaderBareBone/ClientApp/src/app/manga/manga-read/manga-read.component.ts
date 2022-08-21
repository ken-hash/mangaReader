import { error } from '@angular/compiler/src/util';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChapterService } from '../../services/chapter.service';

@Component({
  selector: 'app-manga-read',
  templateUrl: './manga-read.component.html',
  styleUrls: ['./manga-read.component.css']
})
export class MangaReadComponent implements OnInit {
  manga: any;
  chapterName: any;
  chapterDetails: any;
  allChapters: any;
  constructor(private activatedRoute: ActivatedRoute,
    private chapterService: ChapterService, private router: Router) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.manga = params['mangaName'];
      this.chapterName = params['chapterName'];
      this.chapterService.postReadChapter(this.manga, this.chapterName).subscribe(data => {
      }, err => {
        if (err.status == 404)
          this.router.navigate(['/404']);
      });
      this.chapterService.getChapter(this.manga, this.chapterName).subscribe(chapter => this.chapterDetails = chapter);
      this.chapterService.getChapterList(this.manga).subscribe(chapterList => this.allChapters = chapterList);
    });
  }
  ngAfterViewInit() {
    let top = document.getElementById('top');
    if (top !== null) {
      top.scrollIntoView();
      top = null;
    }
  }
}

