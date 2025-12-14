import type { Component } from 'vue';

export interface ClassificationItem {
  id: number;
  icon:  Component; 
  image: string; 
  category: string; 
  title: string;
  accuracy: number; 
  date: string;
  time: string; 
}