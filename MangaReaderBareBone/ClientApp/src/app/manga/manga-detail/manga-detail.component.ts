import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { MangaService } from '../../services/manga.service';

@Component({
  selector: 'app-manga-detail',
  templateUrl: './manga-detail.component.html',
  styleUrls: ['./manga-detail.component.css']
})
export class MangaDetailComponent implements OnInit {
  manga: string | undefined;
  allChapters: any;

  constructor(private activatedRoute: ActivatedRoute,
    private mangaservice: MangaService,
    private router: Router,
    private titleService: Title) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.manga = params['mangaName'];
      this.mangaservice.getMangaDetails(this.manga!).subscribe(detail => { this.allChapters = detail }, err => {
        if (err.status == 404)
          this.router.navigate(['/404']);
      });
      this.titleService.setTitle(`${this.manga?.toUpperCase().replaceAll('-', " ")}`);
    });

  }
}



