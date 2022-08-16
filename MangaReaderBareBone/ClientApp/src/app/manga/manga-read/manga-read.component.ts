import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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
    private chapterService: ChapterService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.manga = params['mangaName'];
      this.chapterName = params['chapterName'];
      this.chapterService.getChapter(this.manga, this.chapterName).subscribe(chapter => this.chapterDetails = chapter);
      this.chapterService.getChapterList(this.manga).subscribe(chapterList => this.allChapters = chapterList);
    });
  }

}
