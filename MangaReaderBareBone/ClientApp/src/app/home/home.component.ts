import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {
  title = 'Manga Reader';
  latestManga: any;
  readManga: any;

  constructor(private http: HttpClient) {

  }
  ngOnInit() {
    this.http.get('https://localhost:7235/api/Latest/manga?numDays=7&numManga=10').subscribe(response => {
      this.latestManga = response;
    }, error => {
      console.log(error);
    })
    this.http.get('https://localhost:7235/api/Latest/mangaRead?numDays=7&numManga=10').subscribe(response => {
      this.readManga = response;
    }, error => {
      console.log(error);
    })
  }
}
