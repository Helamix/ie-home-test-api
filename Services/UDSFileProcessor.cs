namespace ie_home_test_api.Services
{
	public class UDSFileProcessor
	{
		const string _dongleCanId = "00 00 07 E0";
		const string _serviceId = "36"; // Write 05
		const string _negativeResponse = "7F";
		const string _positiveResponse = "76";
		const string _paddingChar = "55";

		public UDSFileProcessor() { }

		public List<byte> ExportWriteData(IFormFile file)
		{
			var writeBlock = ConvertRawFileToBytes(file);
			return writeBlock;
		}

		private List<byte> ConvertRawFileToBytes(IFormFile file)
		{
			var fileContent = new List<byte>();
			var foundData = false;
			using (var fileStream = file.OpenReadStream())
			using (var binaryReader = new BinaryReader(fileStream))
			{
				while (true)
				{
					if (!foundData)
					{
						// Read Timestamp (4 bytes)
						var timestampByte = new byte[4];
						for (int i = 0; i < 4; i++)
						{
							timestampByte[i] = binaryReader.ReadByte();
						}

						// Example: Read CAN ID (4 bytes) - Adjust according to your file format
						var canIdByte = new byte[4];
						for (int i = 0; i < 4; i++)
						{
							canIdByte[i] = binaryReader.ReadByte();
						}
						string canId = BitConverter.ToString(canIdByte).Replace("-", " ");

						// Read PCI (1 byte)
						string PCI = binaryReader.ReadByte().ToString("X2");

						// Read Buffers
						var DID = binaryReader.ReadByte().ToString("X2");
						var DID2 = binaryReader.ReadByte().ToString("X2");

						// Read Service Id
						var serviceId = binaryReader.ReadByte().ToString("X2");

						// Read SBF
						var sbf = binaryReader.ReadByte().ToString("X2");

						// Read Data
						var dataBytes = new byte[4];
						for (int i = 0; i < 4; i++)
						{
							dataBytes[i] = binaryReader.ReadByte();
						}

						string data = BitConverter.ToString(dataBytes).Replace("-", " ");

						if (string.Equals(canId, _dongleCanId, StringComparison.OrdinalIgnoreCase) && string.Equals(serviceId, _serviceId, StringComparison.OrdinalIgnoreCase))
						{
							foundData = true;
							fileContent.AddRange(dataBytes);
						}
					}
					else
					{
						// Read Timestamp (4 bytes)
						var timestampByte = new byte[4];
						for (int i = 0; i < 4; i++)
						{
							timestampByte[i] = binaryReader.ReadByte();
						}

						// Example: Read CAN ID (4 bytes) - Adjust according to your file format
						var canIdByte = new byte[4];
						for (int i = 0; i < 4; i++)
						{
							canIdByte[i] = binaryReader.ReadByte();
						}
						string canId = BitConverter.ToString(canIdByte).Replace("-", " ");

						// Read PCI (1 byte)
						string PCI = binaryReader.ReadByte().ToString("X2");

						// Read Buffers
						var DID = binaryReader.ReadByte().ToString("X2");

						// Read Data
						var dataBytes = new byte[7];
						for (int i = 0; i < 7; i++)
						{
							dataBytes[i] = binaryReader.ReadByte();
						}

						string data = BitConverter.ToString(dataBytes).Replace("-", " ");
						if (string.Equals(canId, _dongleCanId, StringComparison.OrdinalIgnoreCase))
						{
							fileContent.AddRange(dataBytes);
						}
						else
						{
							if (string.Equals(dataBytes[0].ToString("X2"), _negativeResponse, StringComparison.OrdinalIgnoreCase)
								|| string.Equals(dataBytes[0].ToString("X2"), _positiveResponse, StringComparison.OrdinalIgnoreCase))
							{
								// Remove Padding at the end of the transfer block
								for (int i = fileContent.Count - 1; i >= 0; i--)
								{
									var byteString = fileContent[i];
									if (string.Equals(byteString.ToString("X2"), _paddingChar, StringComparison.OrdinalIgnoreCase))
									{
										fileContent.RemoveAt(i);
									}
									else
									{
										break;
									}
								}

								foundData = false;
							}
						}
					}

					if (binaryReader.PeekChar() == -1)
					{
						break;
					}
				}
			}

			return fileContent;
		}
	}
}
