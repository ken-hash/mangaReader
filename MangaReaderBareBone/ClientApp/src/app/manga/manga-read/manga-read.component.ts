import { error } from '@angular/compiler/src/util';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { ChapterService } from '../../services/chapter.service';
import { ChapterDetails } from '../../commons/models/chapterdetails.model';
import { Title } from "@angular/platform-browser";

@Component({
  selector: 'app-manga-read',
  templateUrl: './manga-read.component.html',
  styleUrls: ['./manga-read.component.css']
})
export class MangaReadComponent implements OnInit {
  manga: string | undefined;
  chapterName: string | undefined;
  chapterDetails: ChapterDetails | undefined;
  allChapters: any;
  constructor(private activatedRoute: ActivatedRoute,
    private chapterService: ChapterService, private router: Router, private titleService: Title) {
    router.events.subscribe((event) => { 
      this.activatedRoute.params.subscribe(params => {
        this.manga = params['mangaName'];
        this.chapterName = params['chapterName'];
        this.chapterService.postReadChapter(this.manga!, this.chapterName!).subscribe(data => {
        }, err => {
          if (err.status == 404)
            this.router.navigate(['/404']);
        });
      });
      this.chapterService.getChapter(this.manga!, this.chapterName!).subscribe(chapter => this.chapterDetails = chapter);
    });
  }

  ngOnInit(): void {
    this.chapterService.getChapter(this.manga!, this.chapterName!).subscribe(chapter => this.chapterDetails = chapter);
    this.chapterService.getChapterList(this.manga!).subscribe(chapterList => this.allChapters = chapterList);
  }
  ngAfterViewInit() {
    let top = document.getElementById('top');
    if (top !== null) {
      top.scrollIntoView();
      top = null;
    }
  }

  onChangeObj(chapter: any) {
    this.router.navigate(['Manga',this.manga, chapter]);
  }
}

