import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { MangaService } from '../../services/manga.service';

@Component({
  selector: 'app-manga-search',
  templateUrl: './manga-search.component.html',
  styleUrls: ['./manga-search.component.css']
})
export class MangaSearchComponent implements OnInit {
  keyword: string | undefined;
  allManga: any;

  constructor(
    private activatedRoute: ActivatedRoute,
    private mangaService: MangaService,
    private router: Router,
    private titleService: Title
  ) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.keyword = params['keyword'];
      this.searchManga();
      this.setTitle(this.keyword!);
    });
  }

  private searchManga() {
    this.mangaService.searchKeyword(this.keyword!).subscribe(
      (data: any) => (this.allManga = data),
      (err: { status: number }) => {
        if (err.status == 404) {
          this.router.navigate(['/404']);
        }
      }
    );
  }

  private setTitle(keyword: string) {
    this.titleService.setTitle(keyword);
  }
}
