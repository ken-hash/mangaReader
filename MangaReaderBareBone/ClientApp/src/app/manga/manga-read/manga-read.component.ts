import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChapterService } from '../../services/chapter.service';
import { ChapterDetails } from '../../commons/models/chapterdetails.model';
import { Title } from "@angular/platform-browser";

@Component({
  selector: 'app-manga-read',
  templateUrl: './manga-read.component.html',
  styleUrls: ['./manga-read.component.css']
})
export class MangaReadComponent implements OnInit, AfterViewInit {
  manga: string | undefined;
  chapterName: string | undefined;
  chapterDetails: ChapterDetails | undefined;
  allChapters: any;
  loading: boolean = true;

  constructor(
    private activatedRoute: ActivatedRoute,
    private chapterService: ChapterService,
    private router: Router,
    private titleService: Title
  ) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.manga = params['mangaName'];
      this.chapterName = params['chapterName'];
      this.postReadChapter();
      this.setPageTitle();
      this.getChapterDetails();
      this.getChapterList();
    });
  }

  ngAfterViewInit(): void {
    this.scrollIntoView();
  }

  onChangeObj(chapter: any) {
    this.router.navigate(['Manga', this.manga, chapter]);
  }

  onLoad() {
    this.loading = false;
  }

  private postReadChapter(): void {
    this.chapterService.postReadChapter(this.manga!, this.chapterName!).subscribe(
      () => { },
      (err: { status: number }) => {
        if (err.status == 404) {
          this.router.navigate(['/404']);
        }
      }
    );
  }

  private setPageTitle(): void {
    this.titleService.setTitle(`${this.manga?.toUpperCase().replaceAll("-", " ")} ${this.chapterName}`);
  }

  private getChapterDetails(): void {
    this.chapterService.getChapter(this.manga!, this.chapterName!).subscribe(
      (chapter: ChapterDetails) => {
        this.chapterDetails = chapter;
      },
      (err: { status: number }) => {
        if (err.status == 404) {
          this.router.navigate(['/404']);
        }
      }
    );
  }

  private getChapterList(): void {
    this.chapterService.getChapterList(this.manga!).subscribe(
      (chapterList) => {
        this.allChapters = chapterList;
      }
    );
  }

  private scrollIntoView(): void {
    let top = document.getElementById('top');
    if (top !== null) {
      top.scrollIntoView();
      top = null;
    }
  }
}
