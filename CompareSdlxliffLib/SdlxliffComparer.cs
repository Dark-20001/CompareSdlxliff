using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TextComparer;

namespace CompareSdlxliffLib
{
	public class SdlxliffComparer
	{
		string bmark = "<internal-file form=\"base64\">";
		string bmarkStop = "</internal-file>";

		string xnsdl = "http://sdl.com/FileTypes/SdlXliff/1.0";
		string xnxliff = "urn:oasis:names:tc:xliff:document:1.2";
		TextComparer.TextComparer comparer = new TextComparer.TextComparer();

		public Dictionary<Guid, string> ReadSdlxliff(string file, bool mtOnly,bool isTarget)
		{
			Dictionary<Guid, string> keyValuePairs = new Dictionary<Guid, string>();
			XDocument doc;

			List<string> xlifflines = new List<string>();
			List<string> xlifflinesClean = new List<string>();
			using (StreamReader sr = new StreamReader(file, Encoding.UTF8, true))
			{
				while (!sr.EndOfStream)
				{
					xlifflines.Add(sr.ReadLine());
				}
			}

			string line1 = xlifflines.First();
			List<string> base64lines = new List<string>();

			int s = line1.IndexOf(bmark);

			if (s != -1)
			{
				string lineb = line1.Substring(s + 29);
				base64lines.Add(lineb);

				line1 = line1.Substring(0, s + 29);

				xlifflinesClean.Add(line1);

				int cc = 1;
				for (int i = 1; i < xlifflines.Count; i++)
				{
					if (xlifflines[i].StartsWith(bmarkStop))
					{
						xlifflinesClean.Add(xlifflines[i]);
						cc = i;
						break;
					}
					else
					{
						base64lines.Add((string)xlifflines[i]);
					}
				}
				for (int i = cc+1; i < xlifflines.Count; i++)
				{
					xlifflinesClean.Add(xlifflines[i]);
				}

				doc = XDocument.Parse(String.Join("\r\n", xlifflinesClean));
			}
			else
			{
				doc = XDocument.Parse(string.Join("\r\n",xlifflines));
			}

			
			IEnumerable<XElement> transunits = doc.Descendants(XName.Get("trans-unit", xnxliff));

			foreach (XElement transunit in transunits)
			{
				string content = "";
				string tuid = transunit.Attribute("id").Value;

				XAttribute attTranslate = transunit.Attribute("translate");
				if (attTranslate != null)
				{
					string attTranslateValue = attTranslate.Value;
					if (attTranslateValue == "no")
						continue;
				}

				if (mtOnly)
				{
					IEnumerable<XElement> segElements = transunit.Descendants(XName.Get("seg", xnsdl));
					if (segElements.Count() > 0)
					{
						XElement segElement = segElements.First();
						XAttribute sattOrigin = segElement.Attribute("origin");
						if (sattOrigin != null)
						{
							if (sattOrigin.Value == "mt" || sattOrigin.Value == "nmt")
							{
								if (isTarget)
								{
									IEnumerable<XElement> targetElements = transunit.Descendants(XName.Get("target", xnxliff));
									if (targetElements.Count() > 0)
									{
										XElement targetElement = transunit.Descendants(XName.Get("target", xnxliff)).First();
										IEnumerable<XElement> mrkElementChildren = targetElement.Descendants(XName.Get("mrk", xnxliff));
										if (mrkElementChildren.Count() > 0)
										{
											foreach (XElement mrkElement in mrkElementChildren)
											{
												content += mrkElement.Value + " ";
											}
										}
									}
								}
								else
								{
									IEnumerable<XElement> segSourceElements = transunit.Descendants(XName.Get("seg-source", xnxliff));
									if (segSourceElements.Count() > 0)
									{
										XElement segsourceElement = segSourceElements.First();
										IEnumerable<XElement> mrkElementChildren = segSourceElements.Descendants(XName.Get("mrk", xnxliff));
										if (mrkElementChildren.Count() > 0)
										{
											foreach (XElement mrkElement in mrkElementChildren)
											{
												content += mrkElement.Value + " ";
											}
										}
									}
								}
							}
							else
							{
								IEnumerable<XElement> pOriginElements = transunit.Descendants(XName.Get("prev-origin", xnsdl));
								if (pOriginElements.Count() > 0)
								{
									XElement pOriginElement = pOriginElements.First();
									XAttribute attOrigin = pOriginElement.Attribute("origin");
									if (attOrigin != null)
									{
										if (attOrigin.Value == "mt" || attOrigin.Value == "nmt")
										{
											if (isTarget)
											{
												IEnumerable<XElement> targetElements = transunit.Descendants(XName.Get("target", xnxliff));
												if (targetElements.Count() > 0)
												{
													XElement targetElement = transunit.Descendants(XName.Get("target", xnxliff)).First();
													IEnumerable<XElement> mrkElementChildren = targetElement.Descendants(XName.Get("mrk", xnxliff));
													if (mrkElementChildren.Count() > 0)
													{
														foreach (XElement mrkElement in mrkElementChildren)
														{
															content += mrkElement.Value + " ";
														}
													}
												}
											}
											else
											{
												IEnumerable<XElement> segSourceElements = transunit.Descendants(XName.Get("seg-source", xnxliff));
												if (segSourceElements.Count() > 0)
												{
													XElement segsourceElement = segSourceElements.First();
													IEnumerable<XElement> mrkElementChildren = segSourceElements.Descendants(XName.Get("mrk", xnxliff));
													if (mrkElementChildren.Count() > 0)
													{
														foreach (XElement mrkElement in mrkElementChildren)
														{
															content += mrkElement.Value + " ";
														}
													}
												}
											}
										}
										else
										{
											continue;
										}
									}
									else
									{
										continue;
									}
								}
								else
								{
									continue;
								}
							}
						}
						else
						{
							continue;
						}
					}
					else
					{
						continue;
					}					
				}
				else
				{
					if (isTarget)
					{
						IEnumerable<XElement> targetElements = transunit.Descendants(XName.Get("target", xnxliff));
						if (targetElements.Count() > 0)
						{
							XElement targetElement = transunit.Descendants(XName.Get("target", xnxliff)).First();
							IEnumerable<XElement> mrkElementChildren = targetElement.Descendants(XName.Get("mrk", xnxliff));
							if (mrkElementChildren.Count() > 0)
							{
								foreach (XElement mrkElement in mrkElementChildren)
								{
									content += mrkElement.Value + " ";
								}
							}
						}
					}
					else
					{
						IEnumerable<XElement> segSourceElements = transunit.Descendants(XName.Get("seg-source", xnxliff));
						if (segSourceElements.Count() > 0)
						{
							XElement segsourceElement = segSourceElements.First();
							IEnumerable<XElement> mrkElementChildren = segSourceElements.Descendants(XName.Get("mrk", xnxliff));
							if (mrkElementChildren.Count() > 0)
							{
								foreach (XElement mrkElement in mrkElementChildren)
								{
									content += mrkElement.Value + " ";
								}
							}
						}
					}
				}		

				if(content != "")
					keyValuePairs.Add(Guid.Parse(tuid), content);
			}

			return keyValuePairs;
		}

		public ChangeRate Compare(string file1, string file2, bool mtOnly, bool isTarget, ComparisonType comparisonType)
		{			
			string reportfile = Path.Combine(Path.GetDirectoryName(file1), Path.GetFileNameWithoutExtension(file1) + "_VS_" + Path.GetFileNameWithoutExtension(file2) + ".htm");
			StringBuilder stringBuilder = new StringBuilder();	

			Dictionary<Guid, string> keyValuePairs1 = ReadSdlxliff(file1, mtOnly, isTarget);
			Dictionary<Guid, string> keyValuePairs2 = ReadSdlxliff(file2, mtOnly, isTarget);

			Dictionary<Guid, string> paired1 = new Dictionary<Guid, string>();
			Dictionary<Guid, string> paired2 = new Dictionary<Guid, string>();
			Dictionary<Guid, string> unpaired = new Dictionary<Guid, string>();

			//match
			foreach (KeyValuePair<Guid, string> unit1 in keyValuePairs1)
			{
				IEnumerable<KeyValuePair<Guid,string>> selectedUnits = from unit in keyValuePairs2 where unit.Key == unit1.Key select unit;
				if (selectedUnits.Count() > 0)
				{
					paired1.Add(unit1.Key, unit1.Value);
					paired2.Add(unit1.Key, selectedUnits.First().Value);
				}
				else
				{
					unpaired.Add(unit1.Key,unit1.Value);
				}
			}

			
			//reports
			stringBuilder.Append("<!DOCTYPE HTML><html><head><meta charset='utf-8'><title>Sdlxliff Compare Result</title>");
			stringBuilder.Append("<style>");
			stringBuilder.Append(".removed{");
			stringBuilder.Append("color:red;");
			stringBuilder.Append("text-decoration:line-through;");
			stringBuilder.Append("}");
			stringBuilder.Append(".added{");
			stringBuilder.Append("color:blue;");
			stringBuilder.Append("}");
			stringBuilder.Append("</style>");
			stringBuilder.Append("</head><body><table border='1'>");
			stringBuilder.Append("<tr><td colspan='4'>Settings</td></tr>");
			stringBuilder.Append("<tr><td>Compare on Target</td><td colspan='3'>"+ isTarget.ToString() + "</td></tr>");
			stringBuilder.Append("<tr><td>Compare MT Only</td><td colspan='3'>" + mtOnly.ToString() + "</td></tr>");
			stringBuilder.Append("<tr><td colspan='4'>File 1:</td></tr>");
			stringBuilder.Append("<tr><td colspan='4'>"+file1+"</td></tr>");
			stringBuilder.Append("<tr><td>Translation Units Count</td><td colspan='3'>" + keyValuePairs1.Count + "</td></tr>");
			stringBuilder.Append("<tr><td colspan='4'>File 2:</td></tr>");
			stringBuilder.Append("<tr><td colspan='4'>" + file2 + "</td></tr>");
			stringBuilder.Append("<tr><td>Translation Units Count</td><td colspan='3'>" + keyValuePairs2.Count + "</td></tr>");
			stringBuilder.Append("<tr><td>Matched Translation Units Count</td><td colspan='3'>" + paired1.Count + "</td></tr>");
			stringBuilder.Append("<tr><td colspan='4'></td></tr>");
			stringBuilder.Append("<tr><td><b>ID</b></td><td><b>File 1</b></td><td><b>File 2</b></td><td><b>Changes</b></td></tr>");

			int counter = 0;			
			ChangeRate changeRate = new ChangeRate();
			changeRate.Original = 0; changeRate.Added = 0; changeRate.Removed = 0; changeRate.RemovedCount = 0; changeRate.AddedCount = 0;

			foreach (KeyValuePair<Guid, string> unit1 in keyValuePairs1)
			{
				StringBuilder cb = new StringBuilder();
				List<string> list1 = new List<string>();
				List<string> list2 = new List<string>();

				list1.Add(unit1.Value);
				list2.Add(paired2[unit1.Key]);

				List<ComparisonTextUnit> comparisonTextUnits = comparer.GetComparisonTextUnits(list1, list2, comparisonType);

				changeRate.Original += unit1.Value.Length;

				foreach (ComparisonTextUnit u in comparisonTextUnits)
				{
					switch (u.ComparisonTextUnitType)
					{
						case ComparisonTextUnitType.Identical:
							cb.Append(u.Text);
							break;
						case ComparisonTextUnitType.Removed:
							cb.Append("<span class='removed'>");
							cb.Append(u.Text);
							cb.Append("</span>");
							changeRate.Removed += u.Text.Length;
							changeRate.RemovedCount++;
							break;
						case ComparisonTextUnitType.New:
							cb.Append("<span class='added'>");
							cb.Append(u.Text);
							cb.Append("</span>");
							changeRate.Added += u.Text.Length;
							changeRate.AddedCount++;
							break;
						default:
							break;
					}
				}

				stringBuilder.Append("<tr><td>"+ unit1.Key + "</td><td>"+
					unit1.Value + "</td><td>"+ paired2[unit1.Key] + "</td><td>"+
					cb.ToString()
					+ "</tr>");

				counter++;
			}

			stringBuilder.Append("<tr><td colspan='4'></td></tr>");
			if (changeRate.Original != 0)
			{
				stringBuilder.Append("<tr><td>Added Characters in Total</td><td>" + changeRate.Added + "</td><td colspan='2'>" + Math.Round((decimal)changeRate.Added / changeRate.Original * 100, 2) + "%</td></tr>");
				stringBuilder.Append("<tr><td>Removed Characters in Total</td><td>" + changeRate.Removed + "</td><td colspan='2'>" + Math.Round((decimal)changeRate.Removed / changeRate.Original * 100, 2) + "%</td></tr>");
			}
			if (unpaired.Count > 0)
			{
				stringBuilder.Append("<tr><td colspan='4'>Unmatched Translation Units</td></tr>");
				stringBuilder.Append("<tr><td><b>ID</b></td><td colspan='3'><b>File 1</b></td></tr>");

				foreach (KeyValuePair<Guid, string> unit1 in unpaired)
				{
					stringBuilder.Append("<tr><td>"+unit1.Key.ToString()+"</td><td colspan='2'>"+unit1.Value+"</td></tr>");
				}
			}
			stringBuilder.Append("</table></body></html>");

			if(File.Exists(reportfile))
				File.Delete(reportfile);
			using (StreamWriter streamWriter = new StreamWriter(reportfile, false, Encoding.UTF8))
			{
				streamWriter.Write(stringBuilder.ToString());
			}

				return changeRate;
		}

	}
}
