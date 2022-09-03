import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NavigationStart, Router } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Manga Reader';
  constructor(private titleService: Title, private router: Router) {
  }
  ngOnInit() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationStart))
      .subscribe(event => this.titleService.setTitle('Manga Reader'));
  }
}
