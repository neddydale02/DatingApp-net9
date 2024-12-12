import { RouterLink, RouterLinkActive, RouterModule, RouterOutlet, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { Component, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'members', component: MemberListComponent},
    {path: 'members/:id', component: MemberDetailComponent},
    {path: 'lists', component: ListsComponent},
    {path: 'messages', component: MessagesComponent},
    {path: '**', component: HomeComponent, pathMatch: 'full'},
]